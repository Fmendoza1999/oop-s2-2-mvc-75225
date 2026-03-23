using System.ComponentModel.DataAnnotations;

namespace FoodSafetyTracker.Models
{
    public class FollowUp
    {
        public int Id { get; set; }

        public int InspectionId { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public string Status { get; set; } = "Open"; // Open / Closed

        public DateTime? ClosedDate { get; set; }

        // Navigation property
        public Inspection Inspection { get; set; } = null!;
    }
}