using FarmShield.Models.Entities;
using FarmShield.Models.Enums;

namespace FarmShield.Services
{
    public interface IBiosecurityService
    {
        (double compliancePercentage, RiskLevel riskLevel, bool certified) CalculateCompliance(int compliantCount, int totalCount, int notApplicableCount);
        Task<string> GenerateInspectionCodeAsync();
        Task<string> GenerateFarmCodeAsync(FarmType farmType);
        Task LogActivityAsync(int farmId, BiosecurityCategory category, string activityType, string responsiblePerson, string description, ComplianceStatus complianceStatus, string? remarks = null);
    }
}

