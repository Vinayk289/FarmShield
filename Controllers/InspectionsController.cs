using FarmShield.Data;
using FarmShield.Models.Entities;
using FarmShield.Models.Enums;
using FarmShield.Services;
using FarmShield.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmShield.Controllers
{
    [Authorize]
    public class InspectionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBiosecurityService _biosecurityService;
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public InspectionsController(
            ApplicationDbContext context,
            IBiosecurityService biosecurityService,
            INotificationService notificationService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _biosecurityService = biosecurityService;
            _notificationService = notificationService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? farmId, InspectionStatus? statusFilter, RiskLevel? riskFilter)
        {
            var query = _context.Inspections
                .Include(i => i.Farm)
                .Include(i => i.Inspector)
                .AsQueryable();

            if (farmId.HasValue) query = query.Where(i => i.FarmId == farmId.Value);
            if (statusFilter.HasValue) query = query.Where(i => i.Status == statusFilter.Value);
            if (riskFilter.HasValue) query = query.Where(i => i.RiskLevel == riskFilter.Value);

            var inspections = await query.OrderByDescending(i => i.InspectionDate).ToListAsync();
            var availableFarms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();

            var completedList = inspections.Where(i => i.Status == InspectionStatus.Completed).ToList();
            double avgScore = completedList.Any() ? Math.Round(completedList.Average(i => i.CompliancePercentage), 1) : 100.0;
            int certifiedCount = completedList.Count(i => i.CertifiedBiosecure);

            var model = new InspectionListViewModel
            {
                Inspections = inspections,
                SelectedFarmId = farmId,
                AvailableFarms = availableFarms,
                StatusFilter = statusFilter,
                RiskFilter = riskFilter,
                AverageComplianceScore = avgScore,
                CertifiedFarmsCount = certifiedCount
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var inspection = await _context.Inspections
                .Include(i => i.Farm)
                .Include(i => i.Inspector)
                .Include(i => i.Responses)
                    .ThenInclude(r => r.ChecklistItem)
                .Include(i => i.CorrectiveActions)
                .FirstOrDefaultAsync(i => i.InspectionId == id);

            if (inspection == null) return NotFound();

            var model = new InspectionDetailsViewModel
            {
                Inspection = inspection,
                Responses = inspection.Responses.OrderBy(r => r.ChecklistItem?.SortOrder ?? 0).ToList(),
                CorrectiveActions = inspection.CorrectiveActions.ToList()
            };

            return View(model);
        }

        [Authorize(Roles = "Super Admin,Veterinary Officer,Farm Manager")]
        public async Task<IActionResult> Create(int? farmId)
        {
            var user = await _userManager.GetUserAsync(User);
            var checklist = await _context.InspectionChecklistItems
                .Where(c => c.IsActive)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();

            var availableFarms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();

            var model = new InspectionCreateViewModel
            {
                FarmId = farmId ?? (availableFarms.FirstOrDefault()?.FarmId ?? 0),
                AvailableFarms = availableFarms,
                InspectionDate = DateTime.UtcNow.Date,
                InspectorName = user?.FullName ?? "Dr. Elena Rostova",
                InspectorDesignation = user?.Designation ?? "Senior Veterinary Auditor",
                Items = checklist.Select(c => new InspectionChecklistResponseInput
                {
                    ChecklistItemId = c.ItemId,
                    Category = c.Category,
                    ItemTitle = c.ItemTitle,
                    Description = c.Description,
                    StandardReference = c.StandardReference,
                    Status = ComplianceStatus.Compliant
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin,Veterinary Officer,Farm Manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InspectionCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                string inspectionCode = await _biosecurityService.GenerateInspectionCodeAsync();

                int totalCount = model.Items.Count;
                int compliantCount = model.Items.Count(i => i.Status == ComplianceStatus.Compliant);
                int nonCompliantCount = model.Items.Count(i => i.Status == ComplianceStatus.NonCompliant);
                int notApplicableCount = model.Items.Count(i => i.Status == ComplianceStatus.NotApplicable);

                var (compliancePercentage, riskLevel, certified) = _biosecurityService.CalculateCompliance(compliantCount, totalCount, notApplicableCount);

                var inspection = new Inspection
                {
                    FarmId = model.FarmId,
                    InspectionCode = inspectionCode,
                    InspectionDate = model.InspectionDate,
                    InspectorId = user?.Id ?? "DefaultInspector",
                    InspectorName = model.InspectorName,
                    InspectorDesignation = model.InspectorDesignation,
                    TotalChecklistItems = totalCount,
                    CompliantItems = compliantCount,
                    NonCompliantItems = nonCompliantCount,
                    NotApplicableItems = notApplicableCount,
                    CompliancePercentage = compliancePercentage,
                    RiskLevel = riskLevel,
                    Status = InspectionStatus.Completed,
                    CertifiedBiosecure = certified,
                    ViolationsIdentified = model.ViolationsIdentified,
                    Recommendations = model.Recommendations,
                    CorrectiveActionRequired = model.CorrectiveActionRequired,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Inspections.AddAsync(inspection);
                await _context.SaveChangesAsync();

                // Save Checklist Responses & generate corrective actions for non-compliant items
                var farm = await _context.Farms.FindAsync(model.FarmId);
                foreach (var item in model.Items)
                {
                    var response = new InspectionResponse
                    {
                        InspectionId = inspection.InspectionId,
                        ChecklistItemId = item.ChecklistItemId,
                        Status = item.Status,
                        Remarks = item.Remarks,
                        CorrectiveAction = item.CorrectiveAction
                    };
                    await _context.InspectionResponses.AddAsync(response);

                    if (item.Status == ComplianceStatus.NonCompliant)
                    {
                        var correctiveAction = new CorrectiveAction
                        {
                            FarmId = model.FarmId,
                            InspectionId = inspection.InspectionId,
                            Title = $"Remediate: {item.ItemTitle}",
                            Description = string.IsNullOrWhiteSpace(item.Remarks) ? $"Audit violation identified in {item.Category}." : item.Remarks,
                            AssignedTo = farm?.OwnerName ?? "Farm Manager",
                            DueDate = DateTime.UtcNow.AddDays(7),
                            Status = "Pending"
                        };
                        await _context.CorrectiveActions.AddAsync(correctiveAction);
                    }
                }

                await _context.SaveChangesAsync();

                // Generate system notifications
                var severity = riskLevel == RiskLevel.High ? NotificationSeverity.Danger : (riskLevel == RiskLevel.Medium ? NotificationSeverity.Warning : NotificationSeverity.Success);
                await _notificationService.CreateNotificationAsync(
                    $"Biosecurity Audit Completed: {inspectionCode}",
                    $"Inspection conducted by {model.InspectorName}. Compliance: {compliancePercentage}% ({riskLevel} Risk). Non-compliant items: {nonCompliantCount}.",
                    NotificationCategory.InspectionDue,
                    severity,
                    model.FarmId,
                    $"/Inspections/Details/{inspection.InspectionId}");

                await _biosecurityService.LogActivityAsync(
                    model.FarmId,
                    BiosecurityCategory.AnimalHealthMonitoring,
                    "Digital Biosecurity Audit",
                    model.InspectorName,
                    $"Official biosecurity audit completed: {inspectionCode}. Score: {compliancePercentage}%. Certified: {certified}.",
                    riskLevel == RiskLevel.High ? ComplianceStatus.NonCompliant : ComplianceStatus.Compliant,
                    model.Recommendations);

                TempData["SuccessMessage"] = $"Biosecurity Inspection {inspectionCode} recorded! Compliance: {compliancePercentage}%";
                return RedirectToAction(nameof(Details), new { id = inspection.InspectionId });
            }

            model.AvailableFarms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();
            return View(model);
        }
    }
}

