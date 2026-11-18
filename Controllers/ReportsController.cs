using FarmShield.Data;
using FarmShield.Services;
using FarmShield.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmShield.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IReportService _reportService;

        public ReportsController(ApplicationDbContext context, IReportService reportService)
        {
            _context = context;
            _reportService = reportService;
        }

        public async Task<IActionResult> Index(string reportType = "compliance", int? farmId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var model = new ReportFilterViewModel
            {
                ReportType = reportType.ToLower(),
                FarmId = farmId,
                StartDate = startDate,
                EndDate = endDate,
                AvailableFarms = await _context.Farms.OrderBy(f => f.Name).ToListAsync()
            };

            switch (model.ReportType)
            {
                case "farms":
                    var fQuery = _context.Farms.AsQueryable();
                    if (farmId.HasValue) fQuery = fQuery.Where(f => f.FarmId == farmId.Value);
                    model.Farms = await fQuery.OrderByDescending(f => f.RegistrationDate).ToListAsync();
                    break;

                case "pigs":
                    var pQuery = _context.Animals.Include(a => a.Farm).AsQueryable();
                    if (farmId.HasValue) pQuery = pQuery.Where(a => a.FarmId == farmId.Value);
                    model.Animals = await pQuery.OrderByDescending(a => a.RegistrationDate).ToListAsync();
                    break;

                case "poultry":
                    var pltQuery = _context.PoultryBatches.Include(b => b.Farm).AsQueryable();
                    if (farmId.HasValue) pltQuery = pltQuery.Where(b => b.FarmId == farmId.Value);
                    model.PoultryBatches = await pltQuery.OrderByDescending(b => b.CreatedAt).ToListAsync();
                    break;

                case "compliance":
                case "inspections":
                    var insQuery = _context.Inspections.Include(i => i.Farm).AsQueryable();
                    if (farmId.HasValue) insQuery = insQuery.Where(i => i.FarmId == farmId.Value);
                    if (startDate.HasValue) insQuery = insQuery.Where(i => i.InspectionDate >= startDate.Value);
                    if (endDate.HasValue) insQuery = insQuery.Where(i => i.InspectionDate <= endDate.Value);
                    model.Inspections = await insQuery.OrderByDescending(i => i.InspectionDate).ToListAsync();
                    break;

                case "diseases":
                    var dQuery = _context.DiseaseReports.Include(d => d.Farm).AsQueryable();
                    if (farmId.HasValue) dQuery = dQuery.Where(d => d.FarmId == farmId.Value);
                    if (startDate.HasValue) dQuery = dQuery.Where(d => d.DetectionDate >= startDate.Value);
                    if (endDate.HasValue) dQuery = dQuery.Where(d => d.DetectionDate <= endDate.Value);
                    model.DiseaseReports = await dQuery.OrderByDescending(d => d.DetectionDate).ToListAsync();
                    break;

                case "vaccinations":
                    var vQuery = _context.Vaccinations.Include(v => v.Farm).AsQueryable();
                    if (farmId.HasValue) vQuery = vQuery.Where(v => v.FarmId == farmId.Value);
                    if (startDate.HasValue) vQuery = vQuery.Where(v => v.VaccinationDate >= startDate.Value);
                    if (endDate.HasValue) vQuery = vQuery.Where(v => v.VaccinationDate <= endDate.Value);
                    model.Vaccinations = await vQuery.OrderByDescending(v => v.VaccinationDate).ToListAsync();
                    break;
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ExportCsv(string reportType, int? farmId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var bytes = await _reportService.GenerateCsvReportAsync(reportType, farmId, startDate, endDate);
            string fileName = $"FarmShield_{reportType}_{DateTime.UtcNow:yyyyMMdd_HHmm}.csv";
            return File(bytes, "text/csv", fileName);
        }
    }
}

