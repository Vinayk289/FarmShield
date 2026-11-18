using FarmShield.Data;
using FarmShield.Models.Entities;
using FarmShield.Models.Enums;
using FarmShield.Services;
using FarmShield.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmShield.Controllers
{
    [Authorize]
    public class BiosecurityController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBiosecurityService _biosecurityService;
        private readonly INotificationService _notificationService;

        public BiosecurityController(
            ApplicationDbContext context,
            IBiosecurityService biosecurityService,
            INotificationService notificationService)
        {
            _context = context;
            _biosecurityService = biosecurityService;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index(int? farmId)
        {
            var query = _context.BiosecurityActivities.Include(a => a.Farm).AsQueryable();
            if (farmId.HasValue)
            {
                query = query.Where(a => a.FarmId == farmId.Value);
            }

            var activities = await query.OrderByDescending(a => a.ActivityDate).Take(30).ToListAsync();
            var availableFarms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();

            var model = new BiosecurityDashboardViewModel
            {
                SelectedFarmId = farmId,
                AvailableFarms = availableFarms,
                Activities = activities,
                TotalVisitorsLogged = await _context.Visitors.CountAsync(v => !farmId.HasValue || v.FarmId == farmId.Value),
                TotalCleaningsLogged = await _context.CleaningRecords.CountAsync(c => !farmId.HasValue || c.FarmId == farmId.Value),
                TotalFeedTests = await _context.FeedRecords.CountAsync(f => !farmId.HasValue || f.FarmId == farmId.Value),
                TotalWaterTests = await _context.WaterQualityRecords.CountAsync(w => !farmId.HasValue || w.FarmId == farmId.Value),
                TotalPestChecks = await _context.PestControlRecords.CountAsync(p => !farmId.HasValue || p.FarmId == farmId.Value),
                TotalWasteDisposals = await _context.WasteManagementRecords.CountAsync(wm => !farmId.HasValue || wm.FarmId == farmId.Value)
            };

            return View(model);
        }

        // --- PILLAR A: VISITOR MANAGEMENT ---
        public async Task<IActionResult> Visitors(int? farmId)
        {
            var query = _context.Visitors.Include(v => v.Farm).AsQueryable();
            if (farmId.HasValue) query = query.Where(v => v.FarmId == farmId.Value);

            ViewBag.SelectedFarmId = farmId;
            ViewBag.AvailableFarms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();
            var visitors = await query.OrderByDescending(v => v.EntryTime).ToListAsync();
            return View(visitors);
        }

        public async Task<IActionResult> AddVisitor(int? farmId)
        {
            var farms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();
            var model = new VisitorCreateViewModel
            {
                FarmId = farmId ?? (farms.FirstOrDefault()?.FarmId ?? 0),
                AvailableFarms = farms,
                EntryTime = DateTime.UtcNow
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVisitor(VisitorCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var visitor = new Visitor
                {
                    FarmId = model.FarmId,
                    VisitorName = model.VisitorName,
                    ContactNumber = model.ContactNumber,
                    Organization = model.Organization,
                    PurposeOfVisit = model.PurposeOfVisit,
                    EntryTime = model.EntryTime,
                    VehicleNumber = model.VehicleNumber,
                    VisitedAnotherFarmRecently = model.VisitedAnotherFarmRecently,
                    LastFarmVisitedDetails = model.LastFarmVisitedDetails,
                    DisinfectionConfirmed = model.DisinfectionConfirmed,
                    ShowerTaken = model.ShowerTaken,
                    ProtectiveClothingIssued = model.ProtectiveClothingIssued,
                    ApprovalStatus = model.ApprovalStatus,
                    ApprovedBy = User.Identity?.Name ?? "Gate Biosecurity Officer",
                    Remarks = model.Remarks,
                    RecordedAt = DateTime.UtcNow
                };

                await _context.Visitors.AddAsync(visitor);
                await _context.SaveChangesAsync();

                // High risk alert if visitor visited another farm in past 72h
                if (model.VisitedAnotherFarmRecently)
                {
                    await _notificationService.CreateNotificationAsync(
                        "Elevated Bio-Risk: Visitor with Recent Farm History Logged",
                        $"Visitor '{model.VisitorName}' ({model.Organization}) entered after visiting another farm in past 72 hrs. Vehicle: {model.VehicleNumber ?? "N/A"}.",
                        NotificationCategory.BiosecurityViolation,
                        NotificationSeverity.Warning,
                        model.FarmId,
                        "/Biosecurity/Visitors");
                }

                await _biosecurityService.LogActivityAsync(
                    model.FarmId,
                    BiosecurityCategory.VisitorManagement,
                    "Visitor & Gate Bio-Screening",
                    visitor.ApprovedBy ?? "Biosecurity Staff",
                    $"Gate clearance logged for {visitor.VisitorName} ({visitor.Organization}). Disinfection confirmed: {visitor.DisinfectionConfirmed}.",
                    visitor.VisitedAnotherFarmRecently ? ComplianceStatus.NeedsReview : ComplianceStatus.Compliant,
                    visitor.Remarks);

                TempData["SuccessMessage"] = $"Visitor '{visitor.VisitorName}' recorded and cleared at biosecurity gate.";
                return RedirectToAction(nameof(Visitors), new { farmId = model.FarmId });
            }

            model.AvailableFarms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogVisitorExit(int visitorId)
        {
            var visitor = await _context.Visitors.FindAsync(visitorId);
            if (visitor != null)
            {
                visitor.ExitTime = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Visitor '{visitor.VisitorName}' signed out.";
            }
            return RedirectToAction(nameof(Visitors));
        }

        // --- PILLAR B: CLEANING & DISINFECTION ---
        public async Task<IActionResult> Cleaning(int? farmId)
        {
            var query = _context.CleaningRecords.Include(c => c.Farm).AsQueryable();
            if (farmId.HasValue) query = query.Where(c => c.FarmId == farmId.Value);

            ViewBag.SelectedFarmId = farmId;
            ViewBag.AvailableFarms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();
            var list = await query.OrderByDescending(c => c.CleaningDate).ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> AddCleaning(int? farmId)
        {
            var farms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();
            var model = new CleaningCreateViewModel
            {
                FarmId = farmId ?? (farms.FirstOrDefault()?.FarmId ?? 0),
                AvailableFarms = farms,
                CleaningDate = DateTime.UtcNow.Date,
                PerformedBy = User.Identity?.Name ?? "Sanitation Lead"
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCleaning(CleaningCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var record = new CleaningRecord
                {
                    FarmId = model.FarmId,
                    FacilityArea = model.FacilityArea,
                    EquipmentSanitized = model.EquipmentSanitized,
                    VehicleDisinfectionStation = model.VehicleDisinfectionStation,
                    FootbathRefreshed = model.FootbathRefreshed,
                    HandwashingSuppliesChecked = model.HandwashingSuppliesChecked,
                    DisinfectantUsed = model.DisinfectantUsed,
                    DilutionRatio = model.DilutionRatio,
                    PerformedBy = model.PerformedBy,
                    CleaningDate = model.CleaningDate,
                    Status = "Verified",
                    Notes = model.Notes
                };

                await _context.CleaningRecords.AddAsync(record);
                await _context.SaveChangesAsync();

                await _biosecurityService.LogActivityAsync(
                    model.FarmId,
                    BiosecurityCategory.CleaningDisinfection,
                    "Barn & Equipment Decontamination",
                    model.PerformedBy,
                    $"Sanitized {model.FacilityArea} using {model.DisinfectantUsed} at {model.DilutionRatio}.",
                    ComplianceStatus.Compliant,
                    model.Notes);

                TempData["SuccessMessage"] = "Sanitation and disinfection protocol logged successfully!";
                return RedirectToAction(nameof(Cleaning), new { farmId = model.FarmId });
            }

            model.AvailableFarms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();
            return View(model);
        }

        // --- PILLAR D: FEED & WATER MANAGEMENT ---
        public async Task<IActionResult> FeedWater(int? farmId)
        {
            var feedQuery = _context.FeedRecords.Include(f => f.Farm).AsQueryable();
            var waterQuery = _context.WaterQualityRecords.Include(w => w.Farm).AsQueryable();

            if (farmId.HasValue)
            {
                feedQuery = feedQuery.Where(f => f.FarmId == farmId.Value);
                waterQuery = waterQuery.Where(w => w.FarmId == farmId.Value);
            }

            ViewBag.SelectedFarmId = farmId;
            ViewBag.AvailableFarms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();
            ViewBag.FeedRecords = await feedQuery.OrderByDescending(f => f.InspectionDate).ToListAsync();
            ViewBag.WaterRecords = await waterQuery.OrderByDescending(w => w.TestingDate).ToListAsync();

            return View();
        }

        public async Task<IActionResult> AddFeed(int? farmId)
        {
            var farms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();
            var model = new FeedCreateViewModel
            {
                FarmId = farmId ?? (farms.FirstOrDefault()?.FarmId ?? 0),
                AvailableFarms = farms,
                InspectedBy = User.Identity?.Name ?? "Feed Inspector"
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFeed(FeedCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var record = new FeedRecord
                {
                    FarmId = model.FarmId,
                    InspectionDate = model.InspectionDate,
                    FeedType = model.FeedType,
                    BatchLotNumber = model.BatchLotNumber,
                    SupplierName = model.SupplierName,
                    QuantityKg = model.QuantityKg,
                    QualityCheckPassed = model.QualityCheckPassed,
                    MoistureLevelOk = model.MoistureLevelOk,
                    NoMoldDetected = model.NoMoldDetected,
                    StorageSiloCondition = model.StorageSiloCondition,
                    InspectedBy = model.InspectedBy,
                    Remarks = model.Remarks
                };

                await _context.FeedRecords.AddAsync(record);
                await _context.SaveChangesAsync();

                await _biosecurityService.LogActivityAsync(
                    model.FarmId,
                    BiosecurityCategory.FeedWaterManagement,
                    "Feed Safety Inspection",
                    model.InspectedBy,
                    $"Batch {model.BatchLotNumber} ({model.QuantityKg}kg {model.FeedType}) verified. Moisture & mold check passed: {model.QualityCheckPassed}.",
                    model.QualityCheckPassed ? ComplianceStatus.Compliant : ComplianceStatus.NonCompliant,
                    model.Remarks);

                TempData["SuccessMessage"] = "Feed consignment safety verification recorded!";
                return RedirectToAction(nameof(FeedWater), new { farmId = model.FarmId });
            }

            model.AvailableFarms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> AddWater(int? farmId)
        {
            var farms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();
            var model = new WaterCreateViewModel
            {
                FarmId = farmId ?? (farms.FirstOrDefault()?.FarmId ?? 0),
                AvailableFarms = farms,
                TestedBy = User.Identity?.Name ?? "Water Quality Officer"
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWater(WaterCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var record = new WaterQualityRecord
                {
                    FarmId = model.FarmId,
                    TestingDate = model.TestingDate,
                    WaterSource = model.WaterSource,
                    PhLevel = model.PhLevel,
                    ChlorinePpm = model.ChlorinePpm,
                    TdsLevel = model.TdsLevel,
                    ColiformBacterialTestResult = model.ColiformBacterialTestResult,
                    SanitizationMethod = model.SanitizationMethod,
                    IsPotableSafe = model.IsPotableSafe,
                    TestedBy = model.TestedBy,
                    Remarks = model.Remarks
                };

                await _context.WaterQualityRecords.AddAsync(record);
                await _context.SaveChangesAsync();

                await _biosecurityService.LogActivityAsync(
                    model.FarmId,
                    BiosecurityCategory.FeedWaterManagement,
                    "Drinking Water Chemical & Microbial Test",
                    model.TestedBy,
                    $"Tested {model.WaterSource}: pH {model.PhLevel}, Chlorine {model.ChlorinePpm} ppm, TDS {model.TdsLevel}. Safe: {model.IsPotableSafe}.",
                    model.IsPotableSafe ? ComplianceStatus.Compliant : ComplianceStatus.NonCompliant,
                    model.Remarks);

                TempData["SuccessMessage"] = "Drinking water quality test saved!";
                return RedirectToAction(nameof(FeedWater), new { farmId = model.FarmId });
            }

            model.AvailableFarms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();
            return View(model);
        }

        // --- PILLAR E: PEST & RODENT CONTROL ---
        public async Task<IActionResult> PestControl(int? farmId)
        {
            var query = _context.PestControlRecords.Include(p => p.Farm).AsQueryable();
            if (farmId.HasValue) query = query.Where(p => p.FarmId == farmId.Value);

            ViewBag.SelectedFarmId = farmId;
            ViewBag.AvailableFarms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();
            var list = await query.OrderByDescending(p => p.InspectionDate).ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> AddPestControl(int? farmId)
        {
            var farms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();
            var model = new PestControlCreateViewModel
            {
                FarmId = farmId ?? (farms.FirstOrDefault()?.FarmId ?? 0),
                AvailableFarms = farms,
                InspectorName = User.Identity?.Name ?? "IPM Specialist"
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPestControl(PestControlCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var record = new PestControlRecord
                {
                    FarmId = model.FarmId,
                    InspectionDate = model.InspectionDate,
                    TargetPest = model.TargetPest,
                    BaitStationsChecked = model.BaitStationsChecked,
                    TrapsServiced = model.TrapsServiced,
                    InfestationLevel = model.InfestationLevel,
                    ChemicalOrMethodUsed = model.ChemicalOrMethodUsed,
                    CorrectiveActionTaken = model.CorrectiveActionTaken,
                    NextScheduledDate = model.NextScheduledDate,
                    InspectorName = model.InspectorName,
                    Remarks = model.Remarks
                };

                await _context.PestControlRecords.AddAsync(record);
                await _context.SaveChangesAsync();

                await _biosecurityService.LogActivityAsync(
                    model.FarmId,
                    BiosecurityCategory.PestRodentControl,
                    "Integrated Pest Management Check",
                    model.InspectorName,
                    $"Serviced {model.BaitStationsChecked} bait stations and {model.TrapsServiced} traps. Infestation: {model.InfestationLevel}.",
                    model.InfestationLevel == "High" ? ComplianceStatus.NonCompliant : ComplianceStatus.Compliant,
                    model.CorrectiveActionTaken);

                TempData["SuccessMessage"] = "Pest & rodent surveillance record saved!";
                return RedirectToAction(nameof(PestControl), new { farmId = model.FarmId });
            }

            model.AvailableFarms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();
            return View(model);
        }

        // --- PILLAR F: WASTE MANAGEMENT ---
        public async Task<IActionResult> WasteManagement(int? farmId)
        {
            var query = _context.WasteManagementRecords.Include(w => w.Farm).AsQueryable();
            if (farmId.HasValue) query = query.Where(w => w.FarmId == farmId.Value);

            ViewBag.SelectedFarmId = farmId;
            ViewBag.AvailableFarms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();
            var list = await query.OrderByDescending(w => w.DisposalDate).ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> AddWaste(int? farmId)
        {
            var farms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();
            var model = new WasteCreateViewModel
            {
                FarmId = farmId ?? (farms.FirstOrDefault()?.FarmId ?? 0),
                AvailableFarms = farms,
                SupervisorName = User.Identity?.Name ?? "Bio-Waste Supervisor"
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWaste(WasteCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var record = new WasteManagementRecord
                {
                    FarmId = model.FarmId,
                    DisposalDate = model.DisposalDate,
                    WasteType = model.WasteType,
                    DisposalMethod = model.DisposalMethod,
                    QuantityKg = model.QuantityKg,
                    SanitationOfDisposalAreaChecked = model.SanitationOfDisposalAreaChecked,
                    DisposalDepthOrTemp = model.DisposalDepthOrTemp,
                    SupervisorName = model.SupervisorName,
                    Remarks = model.Remarks
                };

                await _context.WasteManagementRecords.AddAsync(record);
                await _context.SaveChangesAsync();

                await _biosecurityService.LogActivityAsync(
                    model.FarmId,
                    BiosecurityCategory.WasteManagement,
                    "Bio-Waste & Carcass Containment",
                    model.SupervisorName,
                    $"Disposed {model.QuantityKg}kg of {model.WasteType} via {model.DisposalMethod}. Sanitation confirmed.",
                    ComplianceStatus.Compliant,
                    model.Remarks);

                TempData["SuccessMessage"] = "Waste and carcass containment log saved!";
                return RedirectToAction(nameof(WasteManagement), new { farmId = model.FarmId });
            }

            model.AvailableFarms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();
            return View(model);
        }
    }
}

