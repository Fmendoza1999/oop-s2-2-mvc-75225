using FoodSafetyTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FoodSafetyTracker.Controllers
{
    [Authorize]
    public class FollowUpsController : Controller
    {
        private readonly AppDbContext _context;

        public FollowUpsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            Log.Information("FollowUps list viewed by {UserName}", User.Identity!.Name);
            var followUps = await _context.FollowUps
                .Include(f => f.Inspection)
                .ThenInclude(i => i.Premises)
                .OrderBy(f => f.DueDate)
                .ToListAsync();
            return View(followUps);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var followUp = await _context.FollowUps
                .Include(f => f.Inspection)
                .ThenInclude(i => i.Premises)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (followUp == null) return NotFound();
            return View(followUp);
        }

        [Authorize(Roles = "Admin,Inspector")]
        public IActionResult Create()
        {
            ViewBag.InspectionId = new SelectList(_context.Inspections, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Create(FollowUp followUp)
        {
            // Business rule: DueDate cannot be before InspectionDate
            var inspection = await _context.Inspections.FindAsync(followUp.InspectionId);
            if (inspection != null && followUp.DueDate.Date < inspection.InspectionDate.Date)
            {
                Log.Warning(
                    "FollowUp DueDate {DueDate} is before InspectionDate {InspectionDate} for InspectionId={InspectionId}",
                    followUp.DueDate, inspection.InspectionDate, followUp.InspectionId);
                ModelState.AddModelError("DueDate", "Due date cannot be before the inspection date.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(followUp);
                await _context.SaveChangesAsync();
                Log.Information(
                    "FollowUp created: FollowUpId={FollowUpId} InspectionId={InspectionId} DueDate={DueDate} by {UserName}",
                    followUp.Id, followUp.InspectionId, followUp.DueDate, User.Identity!.Name);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.InspectionId = new SelectList(_context.Inspections, "Id", "Id", followUp.InspectionId);
            return View(followUp);
        }

        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var followUp = await _context.FollowUps.FindAsync(id);
            if (followUp == null) return NotFound();
            ViewBag.InspectionId = new SelectList(_context.Inspections, "Id", "Id", followUp.InspectionId);
            return View(followUp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Edit(int id, FollowUp followUp)
        {
            if (id != followUp.Id) return NotFound();

            // Business rule: cannot close without a ClosedDate
            if (followUp.Status == "Closed" && followUp.ClosedDate == null)
            {
                Log.Warning("Attempt to close FollowUp Id={FollowUpId} without ClosedDate", id);
                ModelState.AddModelError("ClosedDate", "A closed date is required when status is Closed.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(followUp);
                    await _context.SaveChangesAsync();
                    Log.Information("FollowUp updated: Id={FollowUpId} Status={Status} by {UserName}",
                        followUp.Id, followUp.Status, User.Identity!.Name);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Log.Error(ex, "Concurrency error updating FollowUp Id={FollowUpId}", id);
                    if (!_context.FollowUps.Any(e => e.Id == followUp.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.InspectionId = new SelectList(_context.Inspections, "Id", "Id", followUp.InspectionId);
            return View(followUp);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var followUp = await _context.FollowUps
                .Include(f => f.Inspection)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (followUp == null) return NotFound();
            return View(followUp);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var followUp = await _context.FollowUps.FindAsync(id);
            if (followUp != null)
            {
                _context.FollowUps.Remove(followUp);
                await _context.SaveChangesAsync();
                Log.Information("FollowUp deleted: Id={FollowUpId} by {UserName}",
                    id, User.Identity!.Name);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}