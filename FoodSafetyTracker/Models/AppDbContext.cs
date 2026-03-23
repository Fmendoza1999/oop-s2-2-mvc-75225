using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyTracker.Models
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Premises> Premises { get; set; }
        public DbSet<Inspection> Inspections { get; set; }
        public DbSet<FollowUp> FollowUps { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Premises -> Inspections
            builder.Entity<Inspection>()
                .HasOne(i => i.Premises)
                .WithMany(p => p.Inspections)
                .HasForeignKey(i => i.PremisesId)
                .OnDelete(DeleteBehavior.Cascade);

            // Inspection -> FollowUps
            builder.Entity<FollowUp>()
                .HasOne(f => f.Inspection)
                .WithMany(i => i.FollowUps)
                .HasForeignKey(f => f.InspectionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}