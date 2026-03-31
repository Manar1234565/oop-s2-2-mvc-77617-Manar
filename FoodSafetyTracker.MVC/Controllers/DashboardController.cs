using FoodSafetyTracker.MVC.Data;
using FoodSafetyTracker.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace FoodSafetyTracker.MVC.Controllers
{
    [Authorize]
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

            // 🔍 FILTERS
            if (!string.IsNullOrEmpty(town))
            {
                _logger.LogInformation("Filtering by town: {Town}", town);
                inspections = inspections.Where(i => i.Premises.Town == town);
            }

            if (!string.IsNullOrEmpty(risk))
            {
                _logger.LogInformation("Filtering by risk: {Risk}", risk);
                inspections = inspections.Where(i => i.Premises.RiskRating == risk);
            }

            // 🔥 REPORTING STYLE (single dataset)
            var filteredInspections = await inspections.ToListAsync();

            var inspectionsThisMonth = filteredInspections
                .Count(i => i.InspectionDate >= startOfMonth);

            var failedInspections = filteredInspections
                .Count(i => i.InspectionDate >= startOfMonth && i.Outcome == "Fail");

            var followUpsQuery = _context.FollowUps
    .Include(f => f.Inspection)
    .ThenInclude(i => i.Premises)
    .AsQueryable();

            //  TOWN
            if (!string.IsNullOrEmpty(town))
            {
                followUpsQuery = followUpsQuery
                    .Where(f => f.Inspection.Premises.Town == town);
            }

            // OVERDUE COUNT
            var overdueFollowUps = await followUpsQuery
                .Where(f => f.Status == "Open" && f.DueDate < DateTime.Now)
                .CountAsync();

            _logger.LogInformation(
                "Dashboard stats - Inspections: {Inspections}, Failed: {Failed}, Overdue: {Overdue}",
                inspectionsThisMonth, failedInspections, overdueFollowUps);

            var model = new DashboardViewModel
            {
                InspectionsThisMonth = inspectionsThisMonth,
                FailedInspectionsThisMonth = failedInspections,
                OverdueFollowUps = overdueFollowUps,
                TownFilter = town,
                
            };
            ViewBag.Towns = await _context.Premises
    .Select(p => p.Town)
    .Distinct()
    .ToListAsync();

            return View(model);
        }
    }
}