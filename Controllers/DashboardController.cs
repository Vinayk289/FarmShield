using FarmShield.Models.Entities;
using FarmShield.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FarmShield.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(IDashboardService dashboardService, UserManager<ApplicationUser> userManager)
        {
            _dashboardService = dashboardService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? farmId = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null && user.AssignedFarmId.HasValue && !farmId.HasValue && !User.IsInRole("Super Admin"))
            {
                farmId = user.AssignedFarmId.Value;
            }

            var model = await _dashboardService.GetDashboardDataAsync(farmId);
            return View(model);
        }
    }
}

