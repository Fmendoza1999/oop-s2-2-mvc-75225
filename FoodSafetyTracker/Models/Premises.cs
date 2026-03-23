using System.ComponentModel.DataAnnotations;

namespace FoodSafetyTracker.Models
{
    public class Premises
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(300)]
        public string Address { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Town { get; set; } = string.Empty;

        [Required]
        public string RiskRating { get; set; } = "Low"; // Low / Medium / High

        // Navigation property
        public ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();
    }
}