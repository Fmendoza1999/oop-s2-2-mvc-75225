using FoodSafetyTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FoodSafetyTracker.Controllers
{
    [Authorize]
    public class InspectionsController : Controller
    {
        private readonly AppDbContext _context;

        public InspectionsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            Log.Information("Inspections list viewed by {UserName}", User.Identity!.Name);
            var inspections = await _context.Inspections
                .Include(i => i.Premises)
                .OrderByDescending(i => i.InspectionDate)
                .ToListAsync();
            return View(inspections);
        }

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

        [Authorize(Roles = "Admin,Inspector")]
        public IActionResult Create()
        {
            ViewBag.PremisesId = new SelectList(_context.Premises, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Create(Inspection inspection)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inspection);
                await _context.SaveChangesAsync();
                Log.Information(
                    "Inspection created: InspectionId={InspectionId} PremisesId={PremisesId} Outcome={Outcome} by {UserName}",
                    inspection.Id, inspection.PremisesId, inspection.Outcome, User.Identity!.Name);
                return RedirectToAction(nameof(Index));
            }
            Log.Warning("Inspection creation failed validation. PremisesId={PremisesId}", inspection.PremisesId);
            ViewBag.PremisesId = new SelectList(_context.Premises, "Id", "Name", inspection.PremisesId);
            return View(inspection);
        }

        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var inspection = await _context.Inspections.FindAsync(id);
            if (inspection == null) return NotFound();
            ViewBag.PremisesId = new SelectList(_context.Premises, "Id", "Name", inspection.PremisesId);
            return View(inspection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Edit(int id, Inspection inspection)
        {
            if (id != inspection.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inspection);
                    await _context.SaveChangesAsync();
                    Log.Information("Inspection updated: InspectionId={InspectionId} by {UserName}",
                        inspection.Id, User.Identity!.Name);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Log.Error(ex, "Concurrency error updating Inspection Id={InspectionId}", id);
                    if (!_context.Inspections.Any(e => e.Id == inspection.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.PremisesId = new SelectList(_context.Premises, "Id", "Name", inspection.PremisesId);
            return View(inspection);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var inspection = await _context.Inspections
                .Include(i => i.Premises)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inspection == null) return NotFound();
            return View(inspection);
        }

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