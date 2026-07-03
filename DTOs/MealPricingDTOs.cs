namespace CanteenAPI.DTOs
{
    public class MealPricingResponse
    {
        public string MealType { get; set; } = string.Empty;
        public decimal BaseCost { get; set; }
        public decimal PaneerSurcharge { get; set; }
        public decimal NonVegSurcharge { get; set; }
    }

    public class UpdateMealPricingRequest
    {
        public string MealType { get; set; } = string.Empty;
        public decimal BaseCost { get; set; }
        public decimal PaneerSurcharge { get; set; }
        public decimal NonVegSurcharge { get; set; }
    }
}