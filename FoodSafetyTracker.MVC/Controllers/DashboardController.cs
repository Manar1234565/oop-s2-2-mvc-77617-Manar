using FoodSafetyTracker.MVC.Data;
using FoodSafetyTracker.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace FoodSafetyTracker.MVC.Controllers
{
    [Authorize] // 🔥 FALTA ESTO
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(AppDbContext context, ILogger<DashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string town, string risk)
        {
            _logger.LogInformation("Dashboard accessed");

            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var inspections = _context.Inspections
                .Include(i => i.Premises)
                .AsQueryable();

            if (!string.IsNullOrEmpty(town))
            {
                _logger.LogInformation($"Filtering by town: {town}");
                inspections = inspections.Where(i => i.Premises.Town == town);
            }

            if (!string.IsNullOrEmpty(risk))
            {
                _logger.LogInformation($"Filtering by risk: {risk}");
                inspections = inspections.Where(i => i.Premises.RiskRating == risk);
            }

            var inspectionsThisMonth = await inspections
                .CountAsync(i => i.InspectionDate >= startOfMonth);

            var failedInspections = await inspections
                .CountAsync(i => i.InspectionDate >= startOfMonth && i.Outcome == "Fail");

            var overdueFollowUps = await _context.FollowUps
                .Include(f => f.Inspection)
                .ThenInclude(i => i.Premises)
                .Where(f => f.Status == "Open" && f.DueDate < DateTime.Now)
                .CountAsync();

            _logger.LogInformation($"Dashboard stats - Inspections: {inspectionsThisMonth}, Failed: {failedInspections}, Overdue: {overdueFollowUps}");

            var model = new DashboardViewModel
            {
                InspectionsThisMonth = inspectionsThisMonth,
                FailedInspectionsThisMonth = failedInspections,
                OverdueFollowUps = overdueFollowUps,
                TownFilter = town,
                RiskFilter = risk
            };

            return View(model);
        }
    }
}