namespace CanteenAPI.DTOs
{
    public class MealRequest
    {
        public string MealType { get; set; } = string.Empty;
        public int VegCount { get; set; }
        public int PaneerCount { get; set; }
        public int NonVegCount { get; set; }
        public bool IsSpecialMeal { get; set; } = false;
    }

    public class CreateBookingRequest
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string CanteenLocation { get; set; } = string.Empty;
        public string BookingFor { get; set; } = "Self";
        public int? GuestCount { get; set; }
        public List<MealRequest> Meals { get; set; } = new();
    }

    public class UpdateBookingGroupRequest
    {
        public List<MealRequest> Meals { get; set; } = new();
    }

    public class UpdateBookingRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? CanteenLocation { get; set; }
        public string? MealType { get; set; }
        public string? BookingFor { get; set; }
        public int? GuestCount { get; set; }
        public int? VegCount { get; set; }
        public int? PaneerCount { get; set; }
        public int? NonVegCount { get; set; }
        public bool? IsSpecialMeal { get; set; }
    }

    public class BookingResponse
    {
        public int BookingID { get; set; }
        public Guid BookingGroupID { get; set; }
        public int? EmployeeID { get; set; }
        public int? NewUserID { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string CanteenLocation { get; set; } = string.Empty;
        public string MealType { get; set; } = string.Empty;
        public string BookingFor { get; set; } = string.Empty;
        public int? GuestCount { get; set; }
        public int VegCount { get; set; }
        public int PaneerCount { get; set; }
        public int NonVegCount { get; set; }
        public bool IsSpecialMeal { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool CanModify { get; set; }
        public bool IsCollected { get; set; }
        public DateTime? CollectedAt { get; set; }
        public decimal TotalCost { get; set; }
    }
}