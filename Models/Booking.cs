namespace CanteenAPI.Models
{
    public class Booking
    {
        public int BookingID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string CanteenLocation { get; set; } = string.Empty;
        public string MealType { get; set; } = string.Empty;
        public string BookingFor { get; set; } = "Self";
        public int? GuestCount { get; set; }
        public int VegCount { get; set; }
        public int PaneerCount { get; set; }
        public int NonVegCount { get; set; }
        public bool IsSpecialMeal { get; set; } = false;
        public string Status { get; set; } = "Confirmed";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Employee? Employee { get; set; }
    }
}
