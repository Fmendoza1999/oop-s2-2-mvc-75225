using Microsoft.AspNetCore.Identity;

namespace FoodSafetyTracker.Models
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = serviceProvider.GetRequiredService<AppDbContext>();

            // Seed Roles
            string[] roles = { "Admin", "Inspector", "Viewer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed Admin user
            await CreateUser(userManager, "admin@food.com", "Admin123!", "Admin");
            await CreateUser(userManager, "felix@myemail.com", "MyPassword1", "Admin");
            await CreateUser(userManager, "john@Dorset.com", "MyDorset1999", "Viewer");
            await CreateUser(userManager, "inspector@food.com", "Inspector123!", "Inspector");
            await CreateUser(userManager, "viewer@food.com", "Viewer123!", "Viewer");

            // Seed Premises, Inspections, FollowUps
            if (!context.Premises.Any())
            {
                var premises = new List<Premises>
                {
                    new Premises { Name = "The Golden Spoon", Address = "12 High St", Town = "Dorchester", RiskRating = "High" },
                    new Premises { Name = "Pizza Palace", Address = "45 Mill Rd", Town = "Dorchester", RiskRating = "Medium" },
                    new Premises { Name = "Burger Barn", Address = "7 King St", Town = "Dorchester", RiskRating = "Low" },
                    new Premises { Name = "Sushi World", Address = "23 Park Ave", Town = "Dorchester", RiskRating = "High" },
                    new Premises { Name = "Green Garden Cafe", Address = "88 River Ln", Town = "Weymouth", RiskRating = "Low" },
                    new Premises { Name = "The Chippy", Address = "3 Beach Rd", Town = "Weymouth", RiskRating = "Medium" },
                    new Premises { Name = "Sunrise Bakery", Address = "19 Sand St", Town = "Weymouth", RiskRating = "Low" },
                    new Premises { Name = "Noodle House", Address = "55 West Ave", Town = "Weymouth", RiskRating = "High" },
                    new Premises { Name = "The Old Pub", Address = "1 Elm St", Town = "Bridport", RiskRating = "Medium" },
                    new Premises { Name = "Curry Corner", Address = "34 North Rd", Town = "Bridport", RiskRating = "High" },
                    new Premises { Name = "Sandwich Stop", Address = "67 South St", Town = "Bridport", RiskRating = "Low" },
                    new Premises { Name = "Cafe Delight", Address = "11 East Rd", Town = "Bridport", RiskRating = "Medium" },
                };
                context.Premises.AddRange(premises);
                await context.SaveChangesAsync();

                var now = DateTime.Now;
                var inspections = new List<Inspection>
                {
                    new Inspection { PremisesId = 1, InspectionDate = now.AddDays(-5),  Score = 45, Outcome = "Fail", Notes = "Poor hygiene in kitchen" },
                    new Inspection { PremisesId = 1, InspectionDate = now.AddDays(-60), Score = 72, Outcome = "Pass", Notes = "Improvement noted" },
                    new Inspection { PremisesId = 2, InspectionDate = now.AddDays(-3),  Score = 85, Outcome = "Pass", Notes = "Good standards" },
                    new Inspection { PremisesId = 3, InspectionDate = now.AddDays(-10), Score = 90, Outcome = "Pass", Notes = "Excellent" },
                    new Inspection { PremisesId = 4, InspectionDate = now.AddDays(-2),  Score = 40, Outcome = "Fail", Notes = "Temperature issues" },
                    new Inspection { PremisesId = 4, InspectionDate = now.AddDays(-90), Score = 55, Outcome = "Fail", Notes = "Repeated issues" },
                    new Inspection { PremisesId = 5, InspectionDate = now.AddDays(-7),  Score = 95, Outcome = "Pass", Notes = "Outstanding" },
                    new Inspection { PremisesId = 6, InspectionDate = now.AddDays(-4),  Score = 60, Outcome = "Pass", Notes = "Acceptable" },
                    new Inspection { PremisesId = 7, InspectionDate = now.AddDays(-1),  Score = 88, Outcome = "Pass", Notes = "Very clean" },
                    new Inspection { PremisesId = 8, InspectionDate = now.AddDays(-6),  Score = 35, Outcome = "Fail", Notes = "Serious violations" },
                    new Inspection { PremisesId = 9, InspectionDate = now.AddDays(-8),  Score = 70, Outcome = "Pass", Notes = "Minor issues" },
                    new Inspection { PremisesId = 10, InspectionDate = now.AddDays(-3), Score = 50, Outcome = "Fail", Notes = "Cross contamination risk" },
                    new Inspection { PremisesId = 11, InspectionDate = now.AddDays(-9), Score = 92, Outcome = "Pass", Notes = "Excellent standards" },
                    new Inspection { PremisesId = 12, InspectionDate = now.AddDays(-2), Score = 78, Outcome = "Pass", Notes = "Good overall" },
                    new Inspection { PremisesId = 2, InspectionDate = now.AddDays(-45), Score = 65, Outcome = "Pass", Notes = "Routine check" },
                    new Inspection { PremisesId = 5, InspectionDate = now.AddDays(-30), Score = 91, Outcome = "Pass", Notes = "Consistent quality" },
                    new Inspection { PremisesId = 6, InspectionDate = now.AddDays(-20), Score = 58, Outcome = "Fail", Notes = "Storage problems" },
                    new Inspection { PremisesId = 9, InspectionDate = now.AddDays(-15), Score = 74, Outcome = "Pass", Notes = "Satisfactory" },
                    new Inspection { PremisesId = 10, InspectionDate = now.AddDays(-50), Score = 48, Outcome = "Fail", Notes = "Pest control needed" },
                    new Inspection { PremisesId = 3, InspectionDate = now.AddDays(-25), Score = 87, Outcome = "Pass", Notes = "Well maintained" },
                    new Inspection { PremisesId = 7, InspectionDate = now.AddDays(-40), Score = 80, Outcome = "Pass", Notes = "Good practices" },
                    new Inspection { PremisesId = 8, InspectionDate = now.AddDays(-12), Score = 42, Outcome = "Fail", Notes = "Ongoing issues" },
                    new Inspection { PremisesId = 11, InspectionDate = now.AddDays(-55), Score = 89, Outcome = "Pass", Notes = "High standard" },
                    new Inspection { PremisesId = 12, InspectionDate = now.AddDays(-35), Score = 76, Outcome = "Pass", Notes = "Mostly compliant" },
                    new Inspection { PremisesId = 1, InspectionDate = now.AddDays(-120), Score = 55, Outcome = "Fail", Notes = "Historical fail" },
                };
                context.Inspections.AddRange(inspections);
                await context.SaveChangesAsync();

                var followUps = new List<FollowUp>
                {
                    new FollowUp { InspectionId = 1,  DueDate = now.AddDays(-10), Status = "Open" },
                    new FollowUp { InspectionId = 1,  DueDate = now.AddDays(5),   Status = "Open" },
                    new FollowUp { InspectionId = 5,  DueDate = now.AddDays(-3),  Status = "Open" },
                    new FollowUp { InspectionId = 6,  DueDate = now.AddDays(-15), Status = "Open" },
                    new FollowUp { InspectionId = 10, DueDate = now.AddDays(-8),  Status = "Open" },
                    new FollowUp { InspectionId = 12, DueDate = now.AddDays(-20), Status = "Open" },
                    new FollowUp { InspectionId = 2,  DueDate = now.AddDays(-30), Status = "Closed", ClosedDate = now.AddDays(-25) },
                    new FollowUp { InspectionId = 4,  DueDate = now.AddDays(-5),  Status = "Closed", ClosedDate = now.AddDays(-2) },
                    new FollowUp { InspectionId = 17, DueDate = now.AddDays(-10), Status = "Open" },
                    new FollowUp { InspectionId = 19, DueDate = now.AddDays(-40), Status = "Closed", ClosedDate = now.AddDays(-35) },
                };
                context.FollowUps.AddRange(followUps);
                await context.SaveChangesAsync();
            }
        }

        private static async Task CreateUser(UserManager<IdentityUser> userManager,
            string email, string password, string role)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}