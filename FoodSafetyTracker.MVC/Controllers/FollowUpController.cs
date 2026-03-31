using FoodSafetyTracker.Domain;
using FoodSafetyTracker.MVC.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyTracker.MVC.Controllers
{
    [Authorize]
    public class FollowUpController : Controller
    {
        private readonly AppDbContext _context;

        public FollowUpController(AppDbContext context)
        {
            _context = context;
        }

        // 👀 LISTA
        [Authorize(Roles = "Admin,Inspector,Viewer")]
        public async Task<IActionResult> Index()
        {
            var followUps = await _context.FollowUps
                .Include(f => f.Inspection)
                .ThenInclude(i => i.Premises)
                .ToListAsync();

            return View(followUps);
        }

        // 🔹 GET: Create
        [Authorize(Roles = "Admin,Inspector")]
        public IActionResult Create()
        {
            ViewBag.Inspections = _context.Inspections
                .Include(i => i.Premises)
                .ToList();

            return View();
        }

        // 🔹 POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Create(FollowUp followUp)
        {
            if (ModelState.IsValid)
            {
                if (followUp.Status == "Closed")
                {
                    followUp.ClosedDate = DateTime.Now;
                }
                else
                {
                    followUp.ClosedDate = null;
                }

                _context.FollowUps.Add(followUp);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Inspections = _context.Inspections
                .Include(i => i.Premises)
                .ToList();

            return View(followUp);
        }
    }
}