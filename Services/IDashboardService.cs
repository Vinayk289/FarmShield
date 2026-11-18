using FarmShield.ViewModels;

namespace FarmShield.Services
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardDataAsync(int? farmId = null);
    }
}

