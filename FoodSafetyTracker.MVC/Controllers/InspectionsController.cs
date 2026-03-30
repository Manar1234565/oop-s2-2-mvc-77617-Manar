using FoodSafetyTracker.Domain;
using FoodSafetyTracker.MVC.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FoodSafetyTracker.MVC.Controllers
{
    [Authorize]
    public class InspectionsController : Controller
    {
        private readonly AppDbContext _context;

        public InspectionsController(AppDbContext context)
        {
            _context = context;
        }

        // Everyone can view
        [Authorize(Roles = "Admin,Inspector,Viewer")]
        public async Task<IActionResult> Index()
        {
            Log.Information("Inspections list viewed by {UserName}", User.Identity!.Name);
            var inspections = await _context.Inspections
                .Include(i => i.Premises)
                .OrderByDescending(i => i.InspectionDate)
                .ToListAsync();
            return View(inspections);
        }

        // Details page
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var inspection = await _context.Inspections
                .Include(i => i.Premises)
                .Include(i => i.FollowUps)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inspection == null) return NotFound();
            return View(inspection);
        }

        // GET Create - show empty form
        [Authorize(Roles = "Admin,Inspector")]
        public IActionResult Create()
        {
            ViewBag.Premises = _context.Premises.ToList();
            return View();
        }

        // POST Create - process the form
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Create(Inspection inspection)
        {
            // Remove navigation property - not sent from form
            // causes false validation failure without this line
            ModelState.Remove("Premises");

            // Business rule: Score must be between 0 and 100
            if (inspection.Score < 0 || inspection.Score > 100)
            {
                ModelState.AddModelError("Score", "Score must be between 0 and 100");
                Log.Warning("Invalid score entered: {Score}", inspection.Score);
            }

            if (ModelState.IsValid)
            {
                _context.Inspections.Add(inspection);
                await _context.SaveChangesAsync();
                Log.Information(
                    "Inspection created with ID {Id} for Premises {PremisesId} by {UserName}",
                    inspection.Id, inspection.PremisesId, User.Identity!.Name);
                return RedirectToAction(nameof(Index));
            }

            // Log exactly which fields failed validation
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                if (state!.Errors.Count > 0)
                {
                    Log.Warning("Validation failed for field {Field}: {Error}",
                        key, state.Errors[0].ErrorMessage);
                }
            }

            Log.Warning("Inspection creation failed validation. PremisesId={PremisesId}",
                inspection.PremisesId);
            ViewBag.Premises = _context.Premises.ToList();
            return View(inspection);
        }

        // GET Edit - show pre-filled form
        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var inspection = await _context.Inspections.FindAsync(id);
            if (inspection == null) return NotFound();
            ViewBag.Premises = _context.Premises.ToList();
            return View(inspection);
        }

        // POST Edit - save changes
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Edit(int id, Inspection inspection)
        {
            if (id != inspection.Id) return NotFound();

            // Remove navigation property - not sent from form
            ModelState.Remove("Premises");

            // Business rule: Score must be between 0 and 100
            if (inspection.Score < 0 || inspection.Score > 100)
            {
                ModelState.AddModelError("Score", "Score must be between 0 and 100");
                Log.Warning("Invalid score entered: {Score}", inspection.Score);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inspection);
                    await _context.SaveChangesAsync();
                    Log.Information(
                        "Inspection updated: Id={InspectionId} by {UserName}",
                        inspection.Id, User.Identity!.Name);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Log.Error(ex,
                        "Concurrency error updating Inspection Id={InspectionId}", id);
                    if (!_context.Inspections.Any(e => e.Id == inspection.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Premises = _context.Premises.ToList();
            return View(inspection);
        }

        // GET Delete - show confirmation page
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var inspection = await _context.Inspections
                .Include(i => i.Premises)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inspection == null)
            {
                Log.Warning("Attempt to delete non-existing inspection ID {Id}", id);
                return NotFound();
            }
            return View(inspection);
        }

        // POST Delete - actually delete after confirmation
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inspection = await _context.Inspections.FindAsync(id);
            if (inspection != null)
            {
                _context.Inspections.Remove(inspection);
                await _context.SaveChangesAsync();
                Log.Information("Inspection deleted: Id={InspectionId} by {UserName}",
                    id, User.Identity!.Name);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}