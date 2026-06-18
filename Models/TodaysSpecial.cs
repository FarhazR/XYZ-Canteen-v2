using System.ComponentModel.DataAnnotations;

namespace CanteenAPI.Models
{
    public class TodaysSpecial
    {
        [Key]
        public int SpecialID { get; set; }
        public string SpecialName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public string MealType { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string ApplicableOutlets { get; set; } = string.Empty;
        public int CreatedByEmployeeID { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Employee? CreatedBy { get; set; }
    }
}