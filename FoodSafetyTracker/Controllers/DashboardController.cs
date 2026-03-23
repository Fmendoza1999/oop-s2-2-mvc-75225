using FoodSafetyTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FoodSafetyTracker.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? town, string? riskRating)
        {
            Log.Information("Dashboard viewed by {UserName} with filters Town={Town} RiskRating={RiskRating}",
                User.Identity!.Name, town, riskRating);

            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            // Base query with optional filters
            var inspectionsQuery = _context.Inspections
                .Include(i => i.Premises)
                .Where(i => string.IsNullOrEmpty(town) || i.Premises.Town == town)
                .Where(i => string.IsNullOrEmpty(riskRating) || i.Premises.RiskRating == riskRating);

            // Aggregations
            var inspectionsThisMonth = await inspectionsQuery
                .CountAsync(i => i.InspectionDate >= startOfMonth);

            var failedThisMonth = await inspectionsQuery
                .CountAsync(i => i.InspectionDate >= startOfMonth && i.Outcome == "Fail");

            var overdueFollowUps = await _context.FollowUps
                .Include(f => f.Inspection)
                .ThenInclude(i => i.Premises)
                .Where(f => f.Status == "Open" && f.DueDate < today)
                .Where(f => string.IsNullOrEmpty(town) || f.Inspection.Premises.Town == town)
                .Where(f => string.IsNullOrEmpty(riskRating) || f.Inspection.Premises.RiskRating == riskRating)
                .ToListAsync();

            // Recent failed inspections
            var recentFails = await inspectionsQuery
                .Where(i => i.Outcome == "Fail")
                .OrderByDescending(i => i.InspectionDate)
                .Take(5)
                .ToListAsync();

            // Filter options
            ViewBag.Towns = await _context.Premises
                .Select(p => p.Town)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            ViewBag.SelectedTown = town;
            ViewBag.SelectedRiskRating = riskRating;
            ViewBag.InspectionsThisMonth = inspectionsThisMonth;
            ViewBag.FailedThisMonth = failedThisMonth;
            ViewBag.OverdueFollowUps = overdueFollowUps;
            ViewBag.RecentFails = recentFails;

            return View();
        }
    }
}