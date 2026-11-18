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
    public class DiseasesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly IBiosecurityService _biosecurityService;

        public DiseasesController(
            ApplicationDbContext context,
            INotificationService notificationService,
            IBiosecurityService biosecurityService)
        {
            _context = context;
            _notificationService = notificationService;
            _biosecurityService = biosecurityService;
        }

        public async Task<IActionResult> Index(int? farmId, DiseaseSeverity? severityFilter, DiseaseVerificationStatus? statusFilter)
        {
            var query = _context.DiseaseReports
                .Include(d => d.Farm)
                .Include(d => d.Animal)
                .Include(d => d.PoultryBatch)
                .AsQueryable();

            if (farmId.HasValue) query = query.Where(d => d.FarmId == farmId.Value);
            if (severityFilter.HasValue) query = query.Where(d => d.Severity == severityFilter.Value);
            if (statusFilter.HasValue) query = query.Where(d => d.VerificationStatus == statusFilter.Value);

            var reports = await query.OrderByDescending(d => d.DetectionDate).ToListAsync();
            var availableFarms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();

            var model = new DiseaseListViewModel
            {
                Reports = reports,
                SelectedFarmId = farmId,
                AvailableFarms = availableFarms,
                SeverityFilter = severityFilter,
                StatusFilter = statusFilter,
                TotalActiveOutbreaks = reports.Count(r => (r.Severity == DiseaseSeverity.Critical || r.Severity == DiseaseSeverity.High) && r.VerificationStatus != DiseaseVerificationStatus.Resolved),
                TotalUnderInvestigation = reports.Count(r => r.VerificationStatus == DiseaseVerificationStatus.UnderInvestigation || r.VerificationStatus == DiseaseVerificationStatus.Suspected),
                TotalResolvedCases = reports.Count(r => r.VerificationStatus == DiseaseVerificationStatus.Resolved)
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var report = await _context.DiseaseReports
                .Include(d => d.Farm)
                .Include(d => d.Animal)
                .Include(d => d.PoultryBatch)
                .Include(d => d.CorrectiveActions)
                .FirstOrDefaultAsync(d => d.ReportId == id);

            if (report == null) return NotFound();

            return View(report);
        }

        public async Task<IActionResult> Report(int? farmId)
        {
            var farms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();
            int selectedFarmId = farmId ?? (farms.FirstOrDefault()?.FarmId ?? 0);

            var animals = await _context.Animals
                .Where(a => a.FarmId == selectedFarmId && a.Status == AnimalStatus.Active)
                .OrderBy(a => a.TagNumber)
                .ToListAsync();

            var batches = await _context.PoultryBatches
                .Where(b => b.FarmId == selectedFarmId && b.Status == PoultryBatchStatus.Active)
                .OrderBy(b => b.BatchCode)
                .ToListAsync();

            var model = new DiseaseReportCreateViewModel
            {
                FarmId = selectedFarmId,
                AvailableFarms = farms,
                AvailableAnimals = animals,
                AvailableBatches = batches,
                DetectionDate = DateTime.UtcNow.Date,
                Severity = DiseaseSeverity.Medium
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Report(DiseaseReportCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var report = new DiseaseReport
                {
                    FarmId = model.FarmId,
                    AnimalId = model.AnimalId,
                    PoultryBatchId = model.PoultryBatchId,
                    DiseaseName = model.DiseaseName,
                    DiseaseCategory = model.DiseaseCategory,
                    DetectionDate = model.DetectionDate,
                    Severity = model.Severity,
                    Symptoms = model.Symptoms,
                    AffectedCount = model.AffectedCount,
                    MortalityCount = model.MortalityCount,
                    IsolationImplemented = model.IsolationImplemented,
                    TreatmentGiven = model.TreatmentGiven,
                    VerificationStatus = model.VerificationStatus,
                    VeterinaryNotes = model.VeterinaryNotes,
                    ReportedBy = User.Identity?.Name ?? "Field Reporter",
                    ReportDate = DateTime.UtcNow
                };

                await _context.DiseaseReports.AddAsync(report);
                await _context.SaveChangesAsync();

                // If associated with a specific pig, update health condition
                if (model.AnimalId.HasValue)
                {
                    var animal = await _context.Animals.FindAsync(model.AnimalId.Value);
                    if (animal != null)
                    {
                        animal.HealthCondition = model.IsolationImplemented ? AnimalHealthStatus.Quarantine : AnimalHealthStatus.Sick;
                        await _context.SaveChangesAsync();
                    }
                }

                // Create alert notification
                var notifSeverity = model.Severity switch
                {
                    DiseaseSeverity.Critical => NotificationSeverity.Danger,
                    DiseaseSeverity.High => NotificationSeverity.Danger,
                    DiseaseSeverity.Medium => NotificationSeverity.Warning,
                    _ => NotificationSeverity.Info
                };

                await _notificationService.CreateNotificationAsync(
                    $"Disease Alert [{model.Severity}]: {model.DiseaseName}",
                    $"Reported {model.AffectedCount} affected ({model.MortalityCount} deaths) at farm. Isolation: {model.IsolationImplemented}. Clinical signs: {model.Symptoms}.",
                    NotificationCategory.DiseaseAlert,
                    notifSeverity,
                    model.FarmId,
                    $"/Diseases/Details/{report.ReportId}");

                await _biosecurityService.LogActivityAsync(
                    model.FarmId,
                    BiosecurityCategory.AnimalHealthMonitoring,
                    "Disease Case / Symptom Incident",
                    report.ReportedBy,
                    $"Incident logged: {model.DiseaseName} ({model.Severity} severity). Affected: {model.AffectedCount}. Isolation instituted.",
                    model.Severity == DiseaseSeverity.Critical ? ComplianceStatus.NonCompliant : ComplianceStatus.NeedsReview,
                    model.Symptoms);

                TempData["SuccessMessage"] = $"Disease report logged successfully! Outbreak alert notifications dispatched.";
                return RedirectToAction(nameof(Details), new { id = report.ReportId });
            }

            model.AvailableFarms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();
            model.AvailableAnimals = await _context.Animals.Where(a => a.FarmId == model.FarmId && a.Status == AnimalStatus.Active).ToListAsync();
            model.AvailableBatches = await _context.PoultryBatches.Where(b => b.FarmId == model.FarmId && b.Status == PoultryBatchStatus.Active).ToListAsync();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin,Veterinary Officer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVerification(int reportId, DiseaseVerificationStatus verificationStatus, string veterinaryNotes)
        {
            var report = await _context.DiseaseReports.FindAsync(reportId);
            if (report != null)
            {
                report.VerificationStatus = verificationStatus;
                report.VeterinaryNotes = veterinaryNotes;
                report.VerifiedBy = User.Identity?.Name ?? "Veterinary Officer";
                report.VerificationDate = DateTime.UtcNow;

                if (verificationStatus == DiseaseVerificationStatus.Resolved && report.AnimalId.HasValue)
                {
                    var animal = await _context.Animals.FindAsync(report.AnimalId.Value);
                    if (animal != null)
                    {
                        animal.HealthCondition = AnimalHealthStatus.Healthy;
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Veterinary diagnosis verification updated to '{verificationStatus}'.";
            }

            return RedirectToAction(nameof(Details), new { id = reportId });
        }
    }
}

