using FarmShield.Data;
using FarmShield.Models.Entities;
using FarmShield.Models.Enums;
using FarmShield.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmShield.Controllers
{
    [Authorize]
    public class PigsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PigsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? farmId, string? searchTag, PigCategory? categoryFilter, AnimalHealthStatus? healthFilter)
        {
            var query = _context.Animals.Include(a => a.Farm).AsQueryable();

            if (farmId.HasValue)
            {
                query = query.Where(a => a.FarmId == farmId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTag))
            {
                query = query.Where(a => a.TagNumber.Contains(searchTag) || a.Breed.Contains(searchTag) || a.PenBarnNumber.Contains(searchTag));
            }

            if (categoryFilter.HasValue)
            {
                query = query.Where(a => a.Category == categoryFilter.Value);
            }

            if (healthFilter.HasValue)
            {
                query = query.Where(a => a.HealthCondition == healthFilter.Value);
            }

            var pigs = await query.OrderByDescending(a => a.RegistrationDate).ToListAsync();
            var availableFarms = await _context.Farms.Where(f => f.FarmType == FarmType.Pig || f.FarmType == FarmType.Mixed).OrderBy(f => f.Name).ToListAsync();

            var model = new PigListViewModel
            {
                Pigs = pigs,
                SelectedFarmId = farmId,
                AvailableFarms = availableFarms,
                SearchTag = searchTag,
                CategoryFilter = categoryFilter,
                HealthFilter = healthFilter,
                TotalCount = pigs.Count,
                TotalHealthy = pigs.Count(p => p.HealthCondition == AnimalHealthStatus.Healthy && p.Status == AnimalStatus.Active),
                TotalSickOrQuarantined = pigs.Count(p => (p.HealthCondition == AnimalHealthStatus.Sick || p.HealthCondition == AnimalHealthStatus.Quarantine || p.HealthCondition == AnimalHealthStatus.UnderTreatment) && p.Status == AnimalStatus.Active),
                TotalSows = pigs.Count(p => p.Category == PigCategory.Sow),
                TotalBoars = pigs.Count(p => p.Category == PigCategory.Boar),
                TotalGrowers = pigs.Count(p => p.Category == PigCategory.Grower),
                TotalPiglets = pigs.Count(p => p.Category == PigCategory.Piglet)
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var pig = await _context.Animals
                .Include(a => a.Farm)
                .Include(a => a.Vaccinations)
                .Include(a => a.DiseaseReports)
                .FirstOrDefaultAsync(a => a.AnimalId == id);

            if (pig == null) return NotFound();

            return View(pig);
        }

        public async Task<IActionResult> Create(int? farmId)
        {
            var availableFarms = await _context.Farms
                .Where(f => f.FarmType == FarmType.Pig || f.FarmType == FarmType.Mixed)
                .OrderBy(f => f.Name)
                .ToListAsync();

            var model = new PigCreateEditViewModel
            {
                FarmId = farmId ?? (availableFarms.FirstOrDefault()?.FarmId ?? 0),
                AvailableFarms = availableFarms,
                DateOfBirth = DateTime.UtcNow.AddMonths(-3),
                CurrentWeightKg = 35.0,
                TagNumber = $"PG-{DateTime.UtcNow:MMdd}-{new Random().Next(10, 99)}"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PigCreateEditViewModel model)
        {
            // Check Tag uniqueness within the farm
            if (await _context.Animals.AnyAsync(a => a.FarmId == model.FarmId && a.TagNumber == model.TagNumber))
            {
                ModelState.AddModelError("TagNumber", "A pig with this Tag Number is already registered in this farm.");
            }

            if (ModelState.IsValid)
            {
                var pig = new Animal
                {
                    FarmId = model.FarmId,
                    TagNumber = model.TagNumber,
                    Breed = model.Breed,
                    Category = model.Category,
                    Gender = model.Gender,
                    DateOfBirth = model.DateOfBirth,
                    CurrentWeightKg = model.CurrentWeightKg,
                    HealthCondition = model.HealthCondition,
                    PenBarnNumber = model.PenBarnNumber,
                    MotherTag = model.MotherTag,
                    FatherTag = model.FatherTag,
                    Status = model.Status,
                    Remarks = model.Remarks,
                    RegistrationDate = DateTime.UtcNow
                };

                await _context.Animals.AddAsync(pig);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Pig Tag '{pig.TagNumber}' registered successfully!";
                return RedirectToAction(nameof(Details), new { id = pig.AnimalId });
            }

            model.AvailableFarms = await _context.Farms.Where(f => f.FarmType == FarmType.Pig || f.FarmType == FarmType.Mixed).OrderBy(f => f.Name).ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var pig = await _context.Animals.FindAsync(id);
            if (pig == null) return NotFound();

            var model = new PigCreateEditViewModel
            {
                AnimalId = pig.AnimalId,
                FarmId = pig.FarmId,
                TagNumber = pig.TagNumber,
                Breed = pig.Breed,
                Category = pig.Category,
                Gender = pig.Gender,
                DateOfBirth = pig.DateOfBirth,
                CurrentWeightKg = pig.CurrentWeightKg,
                HealthCondition = pig.HealthCondition,
                PenBarnNumber = pig.PenBarnNumber,
                MotherTag = pig.MotherTag,
                FatherTag = pig.FatherTag,
                Status = pig.Status,
                Remarks = pig.Remarks,
                AvailableFarms = await _context.Farms.Where(f => f.FarmType == FarmType.Pig || f.FarmType == FarmType.Mixed).OrderBy(f => f.Name).ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PigCreateEditViewModel model)
        {
            if (id != model.AnimalId) return BadRequest();

            if (ModelState.IsValid)
            {
                var pig = await _context.Animals.FindAsync(id);
                if (pig == null) return NotFound();

                pig.Breed = model.Breed;
                pig.Category = model.Category;
                pig.Gender = model.Gender;
                pig.DateOfBirth = model.DateOfBirth;
                pig.CurrentWeightKg = model.CurrentWeightKg;
                pig.HealthCondition = model.HealthCondition;
                pig.PenBarnNumber = model.PenBarnNumber;
                pig.MotherTag = model.MotherTag;
                pig.FatherTag = model.FatherTag;
                pig.Status = model.Status;
                pig.Remarks = model.Remarks;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Record for Pig '{pig.TagNumber}' updated successfully!";
                return RedirectToAction(nameof(Details), new { id = pig.AnimalId });
            }

            model.AvailableFarms = await _context.Farms.Where(f => f.FarmType == FarmType.Pig || f.FarmType == FarmType.Mixed).OrderBy(f => f.Name).ToListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordMortality(int id, string mortalityReason)
        {
            var pig = await _context.Animals.FindAsync(id);
            if (pig != null)
            {
                pig.Status = AnimalStatus.Deceased;
                pig.MortalityDate = DateTime.UtcNow;
                pig.MortalityReason = mortalityReason;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Mortality recorded for Pig '{pig.TagNumber}'.";
            }
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}

