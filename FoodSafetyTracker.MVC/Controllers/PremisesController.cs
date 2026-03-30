using FoodSafetyTracker.MVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace FoodSafetyTracker.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PremisesController : Controller
    {
        private readonly AppDbContext _context;

        public PremisesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var premises = await _context.Premises.ToListAsync();
            return View(premises);
        }
    }
}