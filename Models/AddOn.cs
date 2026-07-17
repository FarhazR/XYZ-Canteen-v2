namespace CanteenAPI.Models
{
    public class AddOn
    {
        public int AddOnID { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal CostPerUnit { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}