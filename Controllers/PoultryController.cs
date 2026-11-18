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
    public class PoultryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PoultryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? farmId, PoultryType? typeFilter, PoultryBatchStatus? statusFilter)
        {
            var query = _context.PoultryBatches.Include(b => b.Farm).AsQueryable();

            if (farmId.HasValue)
            {
                query = query.Where(b => b.FarmId == farmId.Value);
            }

            if (typeFilter.HasValue)
            {
                query = query.Where(b => b.PoultryType == typeFilter.Value);
            }

            if (statusFilter.HasValue)
            {
                query = query.Where(b => b.Status == statusFilter.Value);
            }

            var batches = await query.OrderByDescending(b => b.CreatedAt).ToListAsync();
            var availableFarms = await _context.Farms
                .Where(f => f.FarmType == FarmType.Poultry || f.FarmType == FarmType.Mixed)
                .OrderBy(f => f.Name)
                .ToListAsync();

            var model = new PoultryListViewModel
            {
                Batches = batches,
                SelectedFarmId = farmId,
                AvailableFarms = availableFarms,
                TypeFilter = typeFilter,
                StatusFilter = statusFilter,
                TotalBirdsInFlocks = batches.Where(b => b.Status == PoultryBatchStatus.Active).Sum(b => b.CurrentBirdCount),
                TotalBroilerFlocks = batches.Count(b => b.PoultryType == PoultryType.Broiler && b.Status == PoultryBatchStatus.Active),
                TotalLayerFlocks = batches.Count(b => b.PoultryType == PoultryType.Layer && b.Status == PoultryBatchStatus.Active),
                TotalMortalityOverall = batches.Sum(b => b.TotalMortality)
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var batch = await _context.PoultryBatches
                .Include(b => b.Farm)
                .Include(b => b.DailyLogs.OrderByDescending(l => l.LogDate))
                .Include(b => b.Vaccinations)
                .Include(b => b.DiseaseReports)
                .FirstOrDefaultAsync(b => b.BatchId == id);

            if (batch == null) return NotFound();

            return View(batch);
        }

        public async Task<IActionResult> Create(int? farmId)
        {
            var availableFarms = await _context.Farms
                .Where(f => f.FarmType == FarmType.Poultry || f.FarmType == FarmType.Mixed)
                .OrderBy(f => f.Name)
                .ToListAsync();

            var model = new PoultryBatchCreateViewModel
            {
                FarmId = farmId ?? (availableFarms.FirstOrDefault()?.FarmId ?? 0),
                AvailableFarms = availableFarms,
                HatchDate = DateTime.UtcNow.Date,
                BatchCode = $"PB-{DateTime.UtcNow.Year}-{(availableFarms.Count + 1):D2}",
                InitialBirdCount = 10000
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PoultryBatchCreateViewModel model)
        {
            if (await _context.PoultryBatches.AnyAsync(b => b.FarmId == model.FarmId && b.BatchCode == model.BatchCode))
            {
                ModelState.AddModelError("BatchCode", "A poultry batch with this code already exists on this farm.");
            }

            if (ModelState.IsValid)
            {
                var batch = new PoultryBatch
                {
                    FarmId = model.FarmId,
                    BatchCode = model.BatchCode,
                    BatchName = model.BatchName,
                    PoultryType = model.PoultryType,
                    Breed = model.Breed,
                    HatchDate = model.HatchDate,
                    InitialBirdCount = model.InitialBirdCount,
                    CurrentBirdCount = model.InitialBirdCount,
                    HouseNumber = model.HouseNumber,
                    HousingType = model.HousingType,
                    TargetFcr = model.TargetFcr,
                    Status = PoultryBatchStatus.Active,
                    Remarks = model.Remarks,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.PoultryBatches.AddAsync(batch);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Poultry batch '{batch.BatchCode}' placed successfully!";
                return RedirectToAction(nameof(Details), new { id = batch.BatchId });
            }

            model.AvailableFarms = await _context.Farms.Where(f => f.FarmType == FarmType.Poultry || f.FarmType == FarmType.Mixed).OrderBy(f => f.Name).ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> AddDailyLog(int batchId)
        {
            var batch = await _context.PoultryBatches.FindAsync(batchId);
            if (batch == null) return NotFound();

            var model = new DailyLogCreateViewModel
            {
                BatchId = batchId,
                Batch = batch,
                LogDate = DateTime.UtcNow.Date,
                MortalityCount = 0,
                FeedConsumedKg = 450,
                WaterConsumedLiters = 900
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDailyLog(DailyLogCreateViewModel model)
        {
            var batch = await _context.PoultryBatches.FindAsync(model.BatchId);
            if (batch == null) return NotFound();

            if (ModelState.IsValid)
            {
                var log = new PoultryDailyLog
                {
                    BatchId = model.BatchId,
                    LogDate = model.LogDate,
                    MortalityCount = model.MortalityCount,
                    FeedConsumedKg = model.FeedConsumedKg,
                    WaterConsumedLiters = model.WaterConsumedLiters,
                    EggsCollected = model.EggsCollected,
                    DamagedEggs = model.DamagedEggs,
                    AverageWeightGrams = model.AverageWeightGrams,
                    Notes = model.Notes,
                    RecordedBy = User.Identity?.Name ?? "Flock Manager",
                    RecordedAt = DateTime.UtcNow
                };

                await _context.PoultryDailyLogs.AddAsync(log);

                // Update Batch bird count and cumulative mortality
                batch.TotalMortality += model.MortalityCount;
                batch.CurrentBirdCount = Math.Max(0, batch.CurrentBirdCount - model.MortalityCount);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Daily flock log successfully recorded!";
                return RedirectToAction(nameof(Details), new { id = model.BatchId });
            }

            model.Batch = batch;
            return View(model);
        }
    }
}

