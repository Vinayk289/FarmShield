using System.Text;
using FarmShield.Data;
using FarmShield.Models.Entities;
using FarmShield.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace FarmShield.Services
{
    public interface IReportService
    {
        Task<byte[]> GenerateCsvReportAsync(string reportType, int? farmId = null, DateTime? startDate = null, DateTime? endDate = null);
    }

    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<byte[]> GenerateCsvReportAsync(string reportType, int? farmId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var sb = new StringBuilder();

            switch (reportType.ToLower())
            {
                case "farms":
                    sb.AppendLine("Farm Code,Farm Name,Owner,Type,Capacity,Current Status,BioSafety Level,District,State,Registration Date");
                    var farmsQuery = _context.Farms.AsQueryable();
                    if (farmId.HasValue) farmsQuery = farmsQuery.Where(f => f.FarmId == farmId.Value);
                    var farms = await farmsQuery.ToListAsync();
                    foreach (var f in farms)
                    {
                        sb.AppendLine($"\"{f.FarmCode}\",\"{Escape(f.Name)}\",\"{Escape(f.OwnerName)}\",\"{f.FarmType}\",{f.TotalCapacity},\"{f.Status}\",\"{f.BioSafetyLevel}\",\"{Escape(f.District)}\",\"{Escape(f.State)}\",\"{f.RegistrationDate:yyyy-MM-dd}\"");
                    }
                    break;

                case "pigs":
                    sb.AppendLine("Tag Number,Farm,Breed,Category,Gender,Weight (kg),Health Condition,Pen/Barn,Status,Birth Date");
                    var pigsQuery = _context.Animals.Include(a => a.Farm).AsQueryable();
                    if (farmId.HasValue) pigsQuery = pigsQuery.Where(a => a.FarmId == farmId.Value);
                    var pigs = await pigsQuery.ToListAsync();
                    foreach (var p in pigs)
                    {
                        sb.AppendLine($"\"{p.TagNumber}\",\"{Escape(p.Farm?.Name ?? "")}\",\"{Escape(p.Breed)}\",\"{p.Category}\",\"{p.Gender}\",{p.CurrentWeightKg},\"{p.HealthCondition}\",\"{Escape(p.PenBarnNumber)}\",\"{p.Status}\",\"{p.DateOfBirth:yyyy-MM-dd}\"");
                    }
                    break;

                case "poultry":
                    sb.AppendLine("Batch Code,Batch Name,Farm,Poultry Type,Breed,Initial Count,Current Count,Total Mortality,Mortality %,House,Status");
                    var poultryQuery = _context.PoultryBatches.Include(b => b.Farm).AsQueryable();
                    if (farmId.HasValue) poultryQuery = poultryQuery.Where(b => b.FarmId == farmId.Value);
                    var batches = await poultryQuery.ToListAsync();
                    foreach (var b in batches)
                    {
                        double mortPct = b.InitialBirdCount > 0 ? Math.Round(((double)b.TotalMortality / b.InitialBirdCount) * 100.0, 2) : 0;
                        sb.AppendLine($"\"{b.BatchCode}\",\"{Escape(b.BatchName)}\",\"{Escape(b.Farm?.Name ?? "")}\",\"{b.PoultryType}\",\"{Escape(b.Breed)}\",{b.InitialBirdCount},{b.CurrentBirdCount},{b.TotalMortality},{mortPct}%,\"{Escape(b.HouseNumber)}\",\"{b.Status}\"");
                    }
                    break;

                case "inspections":
                    sb.AppendLine("Inspection Code,Farm,Date,Inspector,Compliant,Non-Compliant,Not Applicable,Compliance %,Risk Level,Status,Certified");
                    var insQuery = _context.Inspections.Include(i => i.Farm).AsQueryable();
                    if (farmId.HasValue) insQuery = insQuery.Where(i => i.FarmId == farmId.Value);
                    if (startDate.HasValue) insQuery = insQuery.Where(i => i.InspectionDate >= startDate.Value);
                    if (endDate.HasValue) insQuery = insQuery.Where(i => i.InspectionDate <= endDate.Value);
                    var inspections = await insQuery.ToListAsync();
                    foreach (var ins in inspections)
                    {
                        sb.AppendLine($"\"{ins.InspectionCode}\",\"{Escape(ins.Farm?.Name ?? "")}\",\"{ins.InspectionDate:yyyy-MM-dd}\",\"{Escape(ins.InspectorName)}\",{ins.CompliantItems},{ins.NonCompliantItems},{ins.NotApplicableItems},{ins.CompliancePercentage}%,\"{ins.RiskLevel}\",\"{ins.Status}\",\"{ins.CertifiedBiosecure}\"");
                    }
                    break;

                case "diseases":
                    sb.AppendLine("Disease Name,Farm,Category,Severity,Detection Date,Affected Count,Mortality,Isolation,Status,Verified By");
                    var disQuery = _context.DiseaseReports.Include(d => d.Farm).AsQueryable();
                    if (farmId.HasValue) disQuery = disQuery.Where(d => d.FarmId == farmId.Value);
                    var diseases = await disQuery.ToListAsync();
                    foreach (var d in diseases)
                    {
                        sb.AppendLine($"\"{Escape(d.DiseaseName)}\",\"{Escape(d.Farm?.Name ?? "")}\",\"{d.DiseaseCategory}\",\"{d.Severity}\",\"{d.DetectionDate:yyyy-MM-dd}\",{d.AffectedCount},{d.MortalityCount},\"{d.IsolationImplemented}\",\"{d.VerificationStatus}\",\"{Escape(d.VerifiedBy ?? "Pending")}\"");
                    }
                    break;

                case "vaccinations":
                    sb.AppendLine("Vaccine Name,Disease Targeted,Farm,Batch Lot,Vaccination Date,Next Due Date,Dose,Route,Status,Administered By");
                    var vacQuery = _context.Vaccinations.Include(v => v.Farm).AsQueryable();
                    if (farmId.HasValue) vacQuery = vacQuery.Where(v => v.FarmId == farmId.Value);
                    var vaccinations = await vacQuery.ToListAsync();
                    foreach (var v in vaccinations)
                    {
                        sb.AppendLine($"\"{Escape(v.VaccineName)}\",\"{Escape(v.DiseaseTargeted)}\",\"{Escape(v.Farm?.Name ?? "")}\",\"{v.BatchLotNumber}\",\"{v.VaccinationDate:yyyy-MM-dd}\",\"{(v.NextDueDate.HasValue ? v.NextDueDate.Value.ToString("yyyy-MM-dd") : "N/A")}\",\"{v.DoseAdministered}\",\"{v.Route}\",\"{v.Status}\",\"{Escape(v.AdministeredBy)}\"");
                    }
                    break;

                default:
                    sb.AppendLine("Invalid Report Type Requested.");
                    break;
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private static string Escape(string input)
        {
            return input.Replace("\"", "\"\"");
        }
    }
}

