using FarmShield.Data;
using FarmShield.Models.Entities;
using FarmShield.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace FarmShield.Services
{
    public class BiosecurityService : IBiosecurityService
    {
        private readonly ApplicationDbContext _context;

        public BiosecurityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public (double compliancePercentage, RiskLevel riskLevel, bool certified) CalculateCompliance(int compliantCount, int totalCount, int notApplicableCount)
        {
            int applicableItems = totalCount - notApplicableCount;
            if (applicableItems <= 0)
            {
                return (100.0, RiskLevel.Low, true);
            }

            double percentage = Math.Round(((double)compliantCount / applicableItems) * 100.0, 1);

            RiskLevel risk;
            bool certified;

            if (percentage >= 85.0)
            {
                risk = RiskLevel.Low;
                certified = true;
            }
            else if (percentage >= 70.0)
            {
                risk = RiskLevel.Medium;
                certified = false;
            }
            else
            {
                risk = RiskLevel.High;
                certified = false;
            }

            return (percentage, risk, certified);
        }

        public async Task<string> GenerateInspectionCodeAsync()
        {
            int currentYear = DateTime.UtcNow.Year;
            int count = await _context.Inspections.CountAsync(i => i.InspectionDate.Year == currentYear);
            return $"INS-{currentYear}-{(count + 1):D3}";
        }

        public async Task<string> GenerateFarmCodeAsync(FarmType farmType)
        {
            string prefix = farmType switch
            {
                FarmType.Pig => "FS-PIG",
                FarmType.Poultry => "FS-PLT",
                _ => "FS-MXD"
            };

            int count = await _context.Farms.CountAsync(f => f.FarmType == farmType);
            return $"{prefix}-{(count + 1):D3}";
        }

        public async Task LogActivityAsync(int farmId, BiosecurityCategory category, string activityType, string responsiblePerson, string description, ComplianceStatus complianceStatus, string? remarks = null)
        {
            var activity = new BiosecurityActivity
            {
                FarmId = farmId,
                Category = category,
                ActivityType = activityType,
                ResponsiblePerson = responsiblePerson,
                Description = description,
                ComplianceStatus = complianceStatus,
                Remarks = remarks,
                ActivityDate = DateTime.UtcNow,
                RecordedAt = DateTime.UtcNow
            };

            await _context.BiosecurityActivities.AddAsync(activity);
            await _context.SaveChangesAsync();
        }
    }
}

