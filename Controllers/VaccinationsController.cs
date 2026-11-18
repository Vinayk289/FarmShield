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
    public class VaccinationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly IBiosecurityService _biosecurityService;

        public VaccinationsController(
            ApplicationDbContext context,
            INotificationService notificationService,
            IBiosecurityService biosecurityService)
        {
            _context = context;
            _notificationService = notificationService;
            _biosecurityService = biosecurityService;
        }

        public async Task<IActionResult> Index(int? farmId, VaccinationStatus? statusFilter)
        {
            var query = _context.Vaccinations
                .Include(v => v.Farm)
                .Include(v => v.Animal)
                .Include(v => v.PoultryBatch)
                .AsQueryable();

            if (farmId.HasValue) query = query.Where(v => v.FarmId == farmId.Value);
            if (statusFilter.HasValue) query = query.Where(v => v.Status == statusFilter.Value);

            var list = await query.OrderByDescending(v => v.VaccinationDate).ToListAsync();
            var availableFarms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();

            var now = DateTime.UtcNow;
            var model = new VaccinationListViewModel
            {
                Vaccinations = list,
                SelectedFarmId = farmId,
                AvailableFarms = availableFarms,
                StatusFilter = statusFilter,
                TotalCompleted = list.Count(v => v.Status == VaccinationStatus.Completed),
                TotalScheduled = list.Count(v => v.Status == VaccinationStatus.Scheduled),
                TotalOverdue = list.Count(v => v.Status == VaccinationStatus.Overdue || (v.Status == VaccinationStatus.Scheduled && v.VaccinationDate < now))
            };

            return View(model);
        }

        public async Task<IActionResult> Schedule(int? farmId)
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

            var model = new VaccinationCreateViewModel
            {
                FarmId = selectedFarmId,
                AvailableFarms = farms,
                AvailableAnimals = animals,
                AvailableBatches = batches,
                VaccinationDate = DateTime.UtcNow.Date,
                NextDueDate = DateTime.UtcNow.Date.AddDays(180),
                AdministeredBy = User.Identity?.Name ?? "Veterinary Technician",
                Status = VaccinationStatus.Completed
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Schedule(VaccinationCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var vaccination = new Vaccination
                {
                    FarmId = model.FarmId,
                    AnimalId = model.AnimalId,
                    PoultryBatchId = model.PoultryBatchId,
                    VaccineName = model.VaccineName,
                    DiseaseTargeted = model.DiseaseTargeted,
                    BatchLotNumber = model.BatchLotNumber,
                    VaccinationDate = model.VaccinationDate,
                    NextDueDate = model.NextDueDate,
                    DoseAdministered = model.DoseAdministered,
                    Route = model.Route,
                    AdministeredBy = model.AdministeredBy,
                    Status = model.Status,
                    Remarks = model.Remarks
                };

                await _context.Vaccinations.AddAsync(vaccination);
                await _context.SaveChangesAsync();

                if (model.Status == VaccinationStatus.Scheduled && model.VaccinationDate <= DateTime.UtcNow.AddDays(7))
                {
                    await _notificationService.CreateNotificationAsync(
                        $"Upcoming Vaccination: {model.VaccineName}",
                        $"Scheduled immunization for {model.DiseaseTargeted} on {model.VaccinationDate:MMM dd, yyyy}. Ensure cold-chain maintenance.",
                        NotificationCategory.OverdueVaccination,
                        NotificationSeverity.Info,
                        model.FarmId,
                        "/Vaccinations");
                }

                await _biosecurityService.LogActivityAsync(
                    model.FarmId,
                    BiosecurityCategory.AnimalHealthMonitoring,
                    "Immunization / Vaccination Record",
                    model.AdministeredBy,
                    $"Administered {model.VaccineName} targeting {model.DiseaseTargeted} (Lot: {model.BatchLotNumber}, Route: {model.Route}).",
                    ComplianceStatus.Compliant,
                    model.Remarks);

                TempData["SuccessMessage"] = $"Vaccination record for '{model.VaccineName}' saved successfully!";
                return RedirectToAction(nameof(Index), new { farmId = model.FarmId });
            }

            model.AvailableFarms = await _context.Farms.OrderBy(f => f.Name).ToListAsync();
            model.AvailableAnimals = await _context.Animals.Where(a => a.FarmId == model.FarmId && a.Status == AnimalStatus.Active).ToListAsync();
            model.AvailableBatches = await _context.PoultryBatches.Where(b => b.FarmId == model.FarmId && b.Status == PoultryBatchStatus.Active).ToListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkCompleted(int id)
        {
            var vaccination = await _context.Vaccinations.FindAsync(id);
            if (vaccination != null)
            {
                vaccination.Status = VaccinationStatus.Completed;
                vaccination.VaccinationDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Vaccination '{vaccination.VaccineName}' marked as completed.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

