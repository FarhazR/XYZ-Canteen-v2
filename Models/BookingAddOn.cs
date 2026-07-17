namespace CanteenAPI.Models
{
    public class BookingAddOn
    {
        public int BookingAddOnID { get; set; }
        public int BookingID { get; set; }
        public int AddOnID { get; set; }
        public int Quantity { get; set; }
        public decimal CostPerUnit { get; set; }
        public decimal TotalCost { get; set; }

        public Booking Booking { get; set; } = null!;
        public AddOn AddOn { get; set; } = null!;
    }
}