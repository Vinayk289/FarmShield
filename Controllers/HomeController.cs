using System.Diagnostics;
using FarmShield.Data;
using FarmShield.Models;
using FarmShield.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmShield.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Landing page statistics from database
            ViewBag.TotalFarms = await _context.Farms.CountAsync();
            ViewBag.TotalPigs = await _context.Animals.CountAsync(a => a.Status == AnimalStatus.Active);
            ViewBag.TotalBirds = await _context.PoultryBatches.Where(b => b.Status == PoultryBatchStatus.Active).SumAsync(b => b.CurrentBirdCount);
            ViewBag.TotalAnimals = ((int)ViewBag.TotalPigs) + ((int)ViewBag.TotalBirds);
            ViewBag.TotalInspections = await _context.Inspections.CountAsync();
            
            var completedInspections = await _context.Inspections.Where(i => i.Status == InspectionStatus.Completed).ToListAsync();
            ViewBag.AverageCompliance = completedInspections.Any()
                ? Math.Round(completedInspections.Average(i => i.CompliancePercentage), 1)
                : 94.5;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
