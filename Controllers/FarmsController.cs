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
    public class FarmsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBiosecurityService _biosecurityService;
        private readonly UserManager<ApplicationUser> _userManager;

        public FarmsController(
            ApplicationDbContext context,
            IBiosecurityService biosecurityService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _biosecurityService = biosecurityService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? searchTerm, FarmType? filterType, FarmStatus? filterStatus, int page = 1)
        {
            int pageSize = 6;
            var query = _context.Farms
                .Include(f => f.Animals)
                .Include(f => f.PoultryBatches)
                .Include(f => f.Inspections)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(f => f.Name.Contains(searchTerm) || f.FarmCode.Contains(searchTerm) || f.District.Contains(searchTerm) || f.OwnerName.Contains(searchTerm));
            }

            if (filterType.HasValue)
            {
                query = query.Where(f => f.FarmType == filterType.Value);
            }

            if (filterStatus.HasValue)
            {
                query = query.Where(f => f.Status == filterStatus.Value);
            }

            int totalCount = await query.CountAsync();
            var farms = await query
                .OrderByDescending(f => f.RegistrationDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new FarmListViewModel
            {
                Farms = farms,
                SearchTerm = searchTerm,
                FilterType = filterType,
                FilterStatus = filterStatus,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                TotalCount = totalCount
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var farm = await _context.Farms
                .Include(f => f.Animals)
                .Include(f => f.PoultryBatches)
                .Include(f => f.Inspections)
                .Include(f => f.Visitors)
                .Include(f => f.DiseaseReports)
                .Include(f => f.Vaccinations)
                .FirstOrDefaultAsync(f => f.FarmId == id);

            if (farm == null) return NotFound();

            var recentInspections = await _context.Inspections
                .Where(i => i.FarmId == id)
                .OrderByDescending(i => i.InspectionDate)
                .Take(5)
                .ToListAsync();

            var recentVisitors = await _context.Visitors
                .Where(v => v.FarmId == id)
                .OrderByDescending(v => v.EntryTime)
                .Take(5)
                .ToListAsync();

            var recentActivities = await _context.BiosecurityActivities
                .Where(a => a.FarmId == id)
                .OrderByDescending(a => a.ActivityDate)
                .Take(5)
                .ToListAsync();

            var activeDiseases = await _context.DiseaseReports
                .Where(d => d.FarmId == id && d.VerificationStatus != DiseaseVerificationStatus.Resolved)
                .OrderByDescending(d => d.DetectionDate)
                .ToListAsync();

            var upcomingVaccinations = await _context.Vaccinations
                .Where(v => v.FarmId == id && v.Status == VaccinationStatus.Scheduled)
                .OrderBy(v => v.VaccinationDate)
                .Take(5)
                .ToListAsync();

            var lastInspection = recentInspections.FirstOrDefault(i => i.Status == InspectionStatus.Completed);

            var model = new FarmDetailsViewModel
            {
                Farm = farm,
                TotalPigs = farm.Animals.Count(a => a.Status == AnimalStatus.Active),
                TotalBirds = farm.PoultryBatches.Where(b => b.Status == PoultryBatchStatus.Active).Sum(b => b.CurrentBirdCount),
                ActiveDiseaseAlerts = activeDiseases.Count,
                LatestComplianceRate = lastInspection?.CompliancePercentage ?? 95.0,
                RecentInspections = recentInspections,
                RecentVisitors = recentVisitors,
                RecentActivities = recentActivities,
                ActiveDiseases = activeDiseases,
                UpcomingVaccinations = upcomingVaccinations
            };

            return View(model);
        }

        [Authorize(Roles = "Super Admin,Farm Owner")]
        public IActionResult Create()
        {
            var model = new FarmCreateEditViewModel
            {
                OwnerName = User.Identity?.Name ?? "Farm Owner",
                FarmType = FarmType.Pig,
                Status = FarmStatus.Active,
                BioSafetyLevel = BioSafetyLevel.Level2_Enhanced
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin,Farm Owner")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FarmCreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                string farmCode = await _biosecurityService.GenerateFarmCodeAsync(model.FarmType);

                var farm = new Farm
                {
                    FarmCode = farmCode,
                    Name = model.Name,
                    OwnerId = user?.Id ?? "DefaultOwner",
                    OwnerName = model.OwnerName,
                    FarmType = model.FarmType,
                    Address = model.Address,
                    District = model.District,
                    State = model.State,
                    ContactNumber = model.ContactNumber,
                    Email = model.Email,
                    FarmAreaAcres = model.FarmAreaAcres,
                    TotalCapacity = model.TotalCapacity,
                    Status = model.Status,
                    BioSafetyLevel = model.BioSafetyLevel,
                    Description = model.Description,
                    ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl)
                        ? (model.FarmType == FarmType.Pig
                            ? "https://images.unsplash.com/photo-1516467508483-a7212febe31a?auto=format&fit=crop&w=1200&q=80"
                            : "https://images.unsplash.com/photo-1548550023-2bdb3c5beed7?auto=format&fit=crop&w=1200&q=80")
                        : model.ImageUrl,
                    RegistrationDate = DateTime.UtcNow
                };

                await _context.Farms.AddAsync(farm);
                await _context.SaveChangesAsync();

                await _biosecurityService.LogActivityAsync(
                    farm.FarmId,
                    BiosecurityCategory.VisitorManagement,
                    "New Farm Registered in Biosecurity Portal",
                    user?.FullName ?? "System",
                    $"Registered new {model.FarmType} facility '{model.Name}' with code {farmCode}.",
                    ComplianceStatus.Compliant);

                TempData["SuccessMessage"] = $"Farm '{farm.Name}' registered successfully with code {farmCode}!";
                return RedirectToAction(nameof(Details), new { id = farm.FarmId });
            }

            return View(model);
        }

        [Authorize(Roles = "Super Admin,Farm Owner,Farm Manager")]
        public async Task<IActionResult> Edit(int id)
        {
            var farm = await _context.Farms.FindAsync(id);
            if (farm == null) return NotFound();

            var model = new FarmCreateEditViewModel
            {
                FarmId = farm.FarmId,
                Name = farm.Name,
                OwnerName = farm.OwnerName,
                FarmType = farm.FarmType,
                Address = farm.Address,
                District = farm.District,
                State = farm.State,
                ContactNumber = farm.ContactNumber,
                Email = farm.Email,
                FarmAreaAcres = farm.FarmAreaAcres,
                TotalCapacity = farm.TotalCapacity,
                Status = farm.Status,
                BioSafetyLevel = farm.BioSafetyLevel,
                Description = farm.Description,
                ImageUrl = farm.ImageUrl
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin,Farm Owner,Farm Manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FarmCreateEditViewModel model)
        {
            if (id != model.FarmId) return BadRequest();

            if (ModelState.IsValid)
            {
                var farm = await _context.Farms.FindAsync(id);
                if (farm == null) return NotFound();

                farm.Name = model.Name;
                farm.OwnerName = model.OwnerName;
                farm.FarmType = model.FarmType;
                farm.Address = model.Address;
                farm.District = model.District;
                farm.State = model.State;
                farm.ContactNumber = model.ContactNumber;
                farm.Email = model.Email;
                farm.FarmAreaAcres = model.FarmAreaAcres;
                farm.TotalCapacity = model.TotalCapacity;
                farm.Status = model.Status;
                farm.BioSafetyLevel = model.BioSafetyLevel;
                farm.Description = model.Description;
                if (!string.IsNullOrWhiteSpace(model.ImageUrl))
                {
                    farm.ImageUrl = model.ImageUrl;
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Farm profile for '{farm.Name}' updated successfully!";
                return RedirectToAction(nameof(Details), new { id = farm.FarmId });
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var farm = await _context.Farms.FindAsync(id);
            if (farm != null)
            {
                _context.Farms.Remove(farm);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Farm '{farm.Name}' and related records deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

