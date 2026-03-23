using System.ComponentModel.DataAnnotations;

namespace FoodSafetyTracker.Models
{
    public class Inspection
    {
        public int Id { get; set; }

        public int PremisesId { get; set; }

        [Required]
        public DateTime InspectionDate { get; set; }

        [Range(0, 100)]
        public int Score { get; set; }

        [Required]
        public string Outcome { get; set; } = "Pass"; // Pass / Fail

        public string? Notes { get; set; }

        // Navigation properties
        public Premises Premises { get; set; } = null!;
        public ICollection<FollowUp> FollowUps { get; set; } = new List<FollowUp>();
    }
}