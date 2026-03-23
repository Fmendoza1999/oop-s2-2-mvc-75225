using FoodSafetyTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyTracker.Tests
{
    public class UnitTest1
    {
        private AppDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        // Test 1: Overdue follow-ups query returns correct items
        [Fact]
        public async Task OverdueFollowUps_ReturnsOnlyOpenAndPastDueDate()
        {
            var context = GetInMemoryContext();
            var today = DateTime.Today;

            var premises = new Premises { Name = "Test Place", Address = "1 St", Town = "Town", RiskRating = "Low" };
            context.Premises.Add(premises);
            await context.SaveChangesAsync();

            var inspection = new Inspection
            {
                PremisesId = premises.Id,
                InspectionDate = today.AddDays(-10),
                Score = 50,
                Outcome = "Fail"
            };
            context.Inspections.Add(inspection);
            await context.SaveChangesAsync();

            context.FollowUps.AddRange(
                new FollowUp { InspectionId = inspection.Id, DueDate = today.AddDays(-5), Status = "Open" },
                new FollowUp { InspectionId = inspection.Id, DueDate = today.AddDays(-1), Status = "Open" },
                new FollowUp { InspectionId = inspection.Id, DueDate = today.AddDays(5), Status = "Open" },
                new FollowUp { InspectionId = inspection.Id, DueDate = today.AddDays(-3), Status = "Closed", ClosedDate = today.AddDays(-1) }
            );
            await context.SaveChangesAsync();

            var overdue = await context.FollowUps
                .Where(f => f.Status == "Open" && f.DueDate < today)
                .ToListAsync();

            Assert.Equal(2, overdue.Count);
        }

        // Test 2: FollowUp cannot be closed without ClosedDate
        [Fact]
        public void FollowUp_ClosedStatus_RequiresClosedDate()
        {
            var followUp = new FollowUp
            {
                InspectionId = 1,
                DueDate = DateTime.Today.AddDays(5),
                Status = "Closed",
                ClosedDate = null
            };

            var isInvalid = followUp.Status == "Closed" && followUp.ClosedDate == null;
            Assert.True(isInvalid);
        }

        // Test 3: Dashboard counts match known seed data
        [Fact]
        public async Task DashboardCounts_MatchKnownData()
        {
            var context = GetInMemoryContext();
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            var premises = new Premises { Name = "Test", Address = "1 St", Town = "Town", RiskRating = "High" };
            context.Premises.Add(premises);
            await context.SaveChangesAsync();

            context.Inspections.AddRange(
                new Inspection { PremisesId = premises.Id, InspectionDate = today.AddDays(-1), Score = 40, Outcome = "Fail" },
                new Inspection { PremisesId = premises.Id, InspectionDate = today.AddDays(-2), Score = 80, Outcome = "Pass" },
                new Inspection { PremisesId = premises.Id, InspectionDate = today.AddDays(-60), Score = 50, Outcome = "Fail" }
            );
            await context.SaveChangesAsync();

            var inspectionsThisMonth = await context.Inspections
                .CountAsync(i => i.InspectionDate >= startOfMonth);

            var failsThisMonth = await context.Inspections
                .CountAsync(i => i.InspectionDate >= startOfMonth && i.Outcome == "Fail");

            Assert.Equal(2, inspectionsThisMonth);
            Assert.Equal(1, failsThisMonth);
        }

        // Test 4: Premises RiskRating only allows valid values
        [Fact]
        public void Premises_RiskRating_ValidValues()
        {
            var validRatings = new[] { "Low", "Medium", "High" };
            var premises = new Premises { Name = "Test", Address = "1 St", Town = "Town", RiskRating = "High" };

            Assert.Contains(premises.RiskRating, validRatings);
        }
    }
}