namespace CanteenAPI.Models
{
    public class MealPricing
    {
        public int MealPricingID { get; set; }
        public string MealType { get; set; } = string.Empty;
        public decimal BaseCost { get; set; }
        public decimal PaneerSurcharge { get; set; }
        public decimal NonVegSurcharge { get; set; }
    }
}