using FarmShield.Models.Entities;

namespace FarmShield.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalFarms { get; set; }
        public int PigFarms { get; set; }
        public int PoultryFarms { get; set; }
        public int MixedFarms { get; set; }

        public int TotalAnimals { get; set; }
        public int TotalBirds { get; set; }
        public int TotalPigs { get; set; }

        public int TotalInspections { get; set; }
        public int PendingInspections { get; set; }
        public int CompletedInspections { get; set; }

        public int ActiveDiseaseAlerts { get; set; }
        public double AverageCompliancePercentage { get; set; }

        public int? SelectedFarmId { get; set; }
        public string? SelectedFarmName { get; set; }
        public List<Farm> AvailableFarms { get; set; } = new();

        // Chart Data properties
        public List<string> MonthlyLabels { get; set; } = new();
        public List<int> MonthlyRegistrations { get; set; } = new();

        public List<string> FarmTypeLabels { get; set; } = new();
        public List<int> FarmTypeCounts { get; set; } = new();

        public List<int> MonthlyInspectionsCompleted { get; set; } = new();
        public List<int> MonthlyInspectionsPending { get; set; } = new();

        public List<double> ComplianceTrends { get; set; } = new();

        public int VaccinationsCompleted { get; set; }
        public int VaccinationsScheduled { get; set; }
        public int VaccinationsOverdue { get; set; }

        public List<string> DiseaseSeverityLabels { get; set; } = new();
        public List<int> DiseaseSeverityCounts { get; set; } = new();

        // Activity lists
        public List<BiosecurityActivity> RecentActivities { get; set; } = new();
        public List<Inspection> UpcomingInspections { get; set; } = new();
        public List<DiseaseReport> CriticalAlerts { get; set; } = new();
        public List<Notification> Notifications { get; set; } = new();
    }
}

