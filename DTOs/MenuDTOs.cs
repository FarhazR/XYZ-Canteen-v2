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
        public int DisplayOrder { get; set; }
    }

    public class AddMenuItemRequest
    {
        public string DayOfWeek { get; set; } = string.Empty;
        public string MealType { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class UpdateMenuItemRequest
    {
        public string? ItemName { get; set; }
        public string? Description { get; set; }
        public string? PhotoUrl { get; set; }
        public int? DisplayOrder { get; set; }
    }

    public class MenuPreviewItem
    {
        public string Action { get; set; } = string.Empty;
        public int? MenuItemID { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public string MealType { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }

    public class MenuChangeRequest
    {
        public string Action { get; set; } = string.Empty;
        public int? MenuItemID { get; set; }
        public AddMenuItemRequest? NewItem { get; set; }
        public UpdateMenuItemRequest? UpdatedItem { get; set; }
    }

    public class MenuPreviewRequest
    {
        public List<MenuChangeRequest> Changes { get; set; } = new();
    }   

    public class MenuPreviewResponse
    {
        public List<MenuPreviewItem> Preview { get; set; } = new();
        public string ConfirmationToken { get; set; } = string.Empty;
    }

    public class MenuConfirmRequest
    {
        public string ConfirmationToken { get; set; } = string.Empty;
        public List<MenuChangeRequest> Changes { get; set; } = new();
    }
}
