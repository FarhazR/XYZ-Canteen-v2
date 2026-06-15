namespace CanteenAPI.DTOs
{
    public class CreateSpecialRequest
    {
        public string SpecialName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public string MealType { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public List<string> ApplicableOutlets { get; set; } = new();
    }

    public class UpdateSpecialRequest
    {
        public string? SpecialName { get; set; }
        public string? Description { get; set; }
        public string? PhotoUrl { get; set; }
        public string? MealType { get; set; }
        public DateTime? Date { get; set; }
        public List<string>? ApplicableOutlets { get; set; }
    }

    public class SpecialResponse
    {
        public int SpecialID { get; set; }
        public string SpecialName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public string MealType { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public List<string> ApplicableOutlets { get; set; } = new();
        public string PublishedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
