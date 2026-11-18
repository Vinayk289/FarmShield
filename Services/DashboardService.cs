using FarmShield.Data;
using FarmShield.Models.Entities;
using FarmShield.Models.Enums;
using FarmShield.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FarmShield.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync(int? farmId = null)
        {
            var model = new DashboardViewModel
            {
                SelectedFarmId = farmId,
                AvailableFarms = await _context.Farms.OrderBy(f => f.Name).ToListAsync()
            };

            if (farmId.HasValue)
            {
                var currentFarm = model.AvailableFarms.FirstOrDefault(f => f.FarmId == farmId.Value);
                model.SelectedFarmName = currentFarm?.Name;
            }

            // Base queries
            var farmsQuery = _context.Farms.AsQueryable();
            var animalsQuery = _context.Animals.AsQueryable();
            var batchesQuery = _context.PoultryBatches.AsQueryable();
            var inspectionsQuery = _context.Inspections.AsQueryable();
            var diseasesQuery = _context.DiseaseReports.AsQueryable();
            var vaccinationsQuery = _context.Vaccinations.AsQueryable();
            var activitiesQuery = _context.BiosecurityActivities.Include(a => a.Farm).AsQueryable();
            var notificationsQuery = _context.Notifications.Include(n => n.Farm).AsQueryable();

            if (farmId.HasValue)
            {
                animalsQuery = animalsQuery.Where(a => a.FarmId == farmId.Value);
                batchesQuery = batchesQuery.Where(b => b.FarmId == farmId.Value);
                inspectionsQuery = inspectionsQuery.Where(i => i.FarmId == farmId.Value);
                diseasesQuery = diseasesQuery.Where(d => d.FarmId == farmId.Value);
                vaccinationsQuery = vaccinationsQuery.Where(v => v.FarmId == farmId.Value);
                activitiesQuery = activitiesQuery.Where(a => a.FarmId == farmId.Value);
                notificationsQuery = notificationsQuery.Where(n => n.FarmId == farmId.Value);
            }

            // KPI Metrics from Database
            model.TotalFarms = await farmsQuery.CountAsync();
            model.PigFarms = await farmsQuery.CountAsync(f => f.FarmType == FarmType.Pig);
            model.PoultryFarms = await farmsQuery.CountAsync(f => f.FarmType == FarmType.Poultry);
            model.MixedFarms = await farmsQuery.CountAsync(f => f.FarmType == FarmType.Mixed);

            model.TotalPigs = await animalsQuery.CountAsync(a => a.Status == AnimalStatus.Active);
            model.TotalBirds = await batchesQuery.Where(b => b.Status == PoultryBatchStatus.Active).SumAsync(b => b.CurrentBirdCount);
            model.TotalAnimals = model.TotalPigs + model.TotalBirds;

            model.TotalInspections = await inspectionsQuery.CountAsync();
            model.CompletedInspections = await inspectionsQuery.CountAsync(i => i.Status == InspectionStatus.Completed);
            model.PendingInspections = await inspectionsQuery.CountAsync(i => i.Status == InspectionStatus.Scheduled || i.Status == InspectionStatus.InProgress);

            model.ActiveDiseaseAlerts = await diseasesQuery.CountAsync(d => d.VerificationStatus != DiseaseVerificationStatus.Resolved);

            var completedInspectionsList = await inspectionsQuery.Where(i => i.Status == InspectionStatus.Completed).ToListAsync();
            model.AverageCompliancePercentage = completedInspectionsList.Any()
                ? Math.Round(completedInspectionsList.Average(i => i.CompliancePercentage), 1)
                : 100.0;

            // Chart 1: Farm Types
            model.FarmTypeLabels = new List<string> { "Pig Farms", "Poultry Farms", "Mixed Farms" };
            model.FarmTypeCounts = new List<int> { model.PigFarms, model.PoultryFarms, model.MixedFarms };

            // Chart 2: Monthly Registrations & Inspections (Last 6 months)
            var now = DateTime.UtcNow;
            for (int i = 5; i >= 0; i--)
            {
                var monthDate = now.AddMonths(-i);
                string monthLabel = monthDate.ToString("MMM yyyy");
                model.MonthlyLabels.Add(monthLabel);

                int regCount = await _context.Farms.CountAsync(f => f.RegistrationDate.Year == monthDate.Year && f.RegistrationDate.Month == monthDate.Month);
                model.MonthlyRegistrations.Add(regCount);

                int compCount = await inspectionsQuery.CountAsync(ins => ins.Status == InspectionStatus.Completed && ins.InspectionDate.Year == monthDate.Year && ins.InspectionDate.Month == monthDate.Month);
                model.MonthlyInspectionsCompleted.Add(compCount);

                int pendCount = await inspectionsQuery.CountAsync(ins => ins.Status != InspectionStatus.Completed && ins.InspectionDate.Year == monthDate.Year && ins.InspectionDate.Month == monthDate.Month);
                model.MonthlyInspectionsPending.Add(pendCount);

                var monthAudits = await inspectionsQuery.Where(ins => ins.Status == InspectionStatus.Completed && ins.InspectionDate.Year == monthDate.Year && ins.InspectionDate.Month == monthDate.Month).ToListAsync();
                double avgComp = monthAudits.Any() ? Math.Round(monthAudits.Average(ins => ins.CompliancePercentage), 1) : 92.5;
                model.ComplianceTrends.Add(avgComp);
            }

            // Chart 3: Vaccination Stats
            model.VaccinationsCompleted = await vaccinationsQuery.CountAsync(v => v.Status == VaccinationStatus.Completed);
            model.VaccinationsScheduled = await vaccinationsQuery.CountAsync(v => v.Status == VaccinationStatus.Scheduled);
            model.VaccinationsOverdue = await vaccinationsQuery.CountAsync(v => v.Status == VaccinationStatus.Overdue || (v.Status == VaccinationStatus.Scheduled && v.VaccinationDate < DateTime.UtcNow));

            // Chart 4: Disease Reports by Severity
            model.DiseaseSeverityLabels = new List<string> { "Critical", "High", "Medium", "Low" };
            model.DiseaseSeverityCounts = new List<int>
            {
                await diseasesQuery.CountAsync(d => d.Severity == DiseaseSeverity.Critical),
                await diseasesQuery.CountAsync(d => d.Severity == DiseaseSeverity.High),
                await diseasesQuery.CountAsync(d => d.Severity == DiseaseSeverity.Medium),
                await diseasesQuery.CountAsync(d => d.Severity == DiseaseSeverity.Low)
            };

            // Feed Lists
            model.RecentActivities = await activitiesQuery
                .OrderByDescending(a => a.ActivityDate)
                .Take(6)
                .ToListAsync();

            model.UpcomingInspections = await inspectionsQuery
                .Include(i => i.Farm)
                .OrderBy(i => i.InspectionDate)
                .Take(5)
                .ToListAsync();

            model.CriticalAlerts = await diseasesQuery
                .Include(d => d.Farm)
                .Where(d => d.Severity == DiseaseSeverity.Critical || d.Severity == DiseaseSeverity.High || d.VerificationStatus == DiseaseVerificationStatus.UnderInvestigation)
                .OrderByDescending(d => d.DetectionDate)
                .Take(5)
                .ToListAsync();

            model.Notifications = await notificationsQuery
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .ToListAsync();

            return model;
        }
    }
}

