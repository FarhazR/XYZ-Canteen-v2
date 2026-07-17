namespace CanteenAPI.DTOs
{
    public class AddOnResponse
    {
        public int AddOnID { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal CostPerUnit { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class CreateAddOnRequest
    {
        public string Name { get; set; } = string.Empty;
        public decimal CostPerUnit { get; set; }
    }

    public class UpdateAddOnRequest
    {
        public string? Name { get; set; }
        public decimal? CostPerUnit { get; set; }
        public bool? IsAvailable { get; set; }
    }
}