using FoodSafetyTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FoodSafetyTracker.Controllers
{
    [Authorize]
    public class PremisesController : Controller
    {
        private readonly AppDbContext _context;

        public PremisesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Premises
        public async Task<IActionResult> Index()
        {
            Log.Information("Premises list viewed by {UserName}", User.Identity!.Name);
            return View(await _context.Premises.ToListAsync());
        }

        // GET: Premises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var premises = await _context.Premises
                .Include(p => p.Inspections)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (premises == null) return NotFound();
            return View(premises);
        }

        // GET: Premises/Create
        [Authorize(Roles = "Admin,Inspector")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Premises/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Create(Premises premises)
        {
            if (ModelState.IsValid)
            {
                _context.Add(premises);
                await _context.SaveChangesAsync();
                Log.Information("Premises created: {Name} in {Town} by {UserName}",
                    premises.Name, premises.Town, User.Identity!.Name);
                return RedirectToAction(nameof(Index));
            }
            Log.Warning("Premises creation failed validation for {Name}", premises.Name);
            return View(premises);
        }

        // GET: Premises/Edit/5
        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var premises = await _context.Premises.FindAsync(id);
            if (premises == null) return NotFound();
            return View(premises);
        }

        // POST: Premises/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Edit(int id, Premises premises)
        {
            if (id != premises.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(premises);
                    await _context.SaveChangesAsync();
                    Log.Information("Premises updated: Id={PremisesId} by {UserName}",
                        premises.Id, User.Identity!.Name);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Log.Error(ex, "Concurrency error updating Premises Id={PremisesId}", id);
                    if (!_context.Premises.Any(e => e.Id == premises.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(premises);
        }

        // GET: Premises/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var premises = await _context.Premises.FirstOrDefaultAsync(m => m.Id == id);
            if (premises == null) return NotFound();
            return View(premises);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var premises = await _context.Premises.FindAsync(id);
            if (premises != null)
            {
                _context.Premises.Remove(premises);
                await _context.SaveChangesAsync();
                Log.Information("Premises deleted: Id={PremisesId} by {UserName}",
                    id, User.Identity!.Name);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}