namespace CanteenAPI.DTOs
{
    public class MenuItemResponse
    {
        public int MenuItemID { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public string MealType { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
    }
}
