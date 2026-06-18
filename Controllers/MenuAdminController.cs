using CanteenAPI.Data;
using CanteenAPI.DTOs;
using CanteenAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CanteenAPI.Controllers
{
    [ApiController]
    [Route("api/menu/admin")]
    [Authorize(Roles = "Admin")]
    public class MenuAdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        private static readonly List<string> ValidDays = new()
        {
            "Monday", "Tuesday", "Wednesday",
            "Thursday", "Friday", "Saturday", "Sunday"
        };

        private static readonly List<string> ValidMealTypes = new()
        {
            "Breakfast", "Lunch", "Evening Snacks", "Dinner"
        };

        public MenuAdminController(AppDbContext context)
        {
            _context = context;
        }

        // POST api/menu/admin/preview
        // Admin submits a list of changes and gets back a summary before confirming
        [HttpPost("preview")]
        public IActionResult Preview([FromBody] MenuPreviewRequest request)
        {
            if (request.Changes == null || !request.Changes.Any())
                return BadRequest(new { message = "No changes provided." });

            var preview = new List<MenuPreviewItem>();

            foreach (var change in request.Changes)
            {
                if (change.Action == "Add")
                {
                    if (change.NewItem == null)
                        return BadRequest(new { message = "NewItem is required for Add action." });

                    if (!ValidDays.Contains(change.NewItem.DayOfWeek))
                        return BadRequest(new { message = $"Invalid day: {change.NewItem.DayOfWeek}" });

                    if (!ValidMealTypes.Contains(change.NewItem.MealType))
                        return BadRequest(new { message = $"Invalid meal type: {change.NewItem.MealType}" });

                    preview.Add(new MenuPreviewItem
                    {
                        Action = "Add",
                        DayOfWeek = change.NewItem.DayOfWeek,
                        MealType = change.NewItem.MealType,
                        OldValue = null,
                        NewValue = change.NewItem.ItemName
                    });
                }
                else if (change.Action == "Update")
                {
                    if (change.MenuItemID == null)
                        return BadRequest(new { message = "MenuItemID is required for Update action." });

                    var existing = _context.MenuItems
                        .FirstOrDefault(m => m.MenuItemID == change.MenuItemID);

                    if (existing == null)
                        return NotFound(new { message = $"Menu item {change.MenuItemID} not found." });

                    preview.Add(new MenuPreviewItem
                    {
                        Action = "Update",
                        MenuItemID = existing.MenuItemID,
                        DayOfWeek = existing.DayOfWeek,
                        MealType = existing.MealType,
                        OldValue = existing.ItemName,
                        NewValue = change.UpdatedItem?.ItemName ?? existing.ItemName
                    });
                }
                else if (change.Action == "Delete")
                {
                    if (change.MenuItemID == null)
                        return BadRequest(new { message = "MenuItemID is required for Delete action." });

                    var existing = _context.MenuItems
                        .FirstOrDefault(m => m.MenuItemID == change.MenuItemID);

                    if (existing == null)
                        return NotFound(new { message = $"Menu item {change.MenuItemID} not found." });

                    preview.Add(new MenuPreviewItem
                    {
                        Action = "Delete",
                        MenuItemID = existing.MenuItemID,
                        DayOfWeek = existing.DayOfWeek,
                        MealType = existing.MealType,
                        OldValue = existing.ItemName,
                        NewValue = null
                    });
                }
                else
                {
                    return BadRequest(new { message = $"Invalid action: {change.Action}. Use Add, Update, or Delete." });
                }
            }

            // Generate a confirmation token by serializing the changes
            var token = Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(
                    JsonSerializer.Serialize(request.Changes)));

            return Ok(new MenuPreviewResponse
            {
                Preview = preview,
                ConfirmationToken = token
            });
        }

        // POST api/menu/admin/confirm
        // Admin confirms the previewed changes — this is where they actually apply
        [HttpPost("confirm")]
        public IActionResult Confirm([FromBody] MenuConfirmRequest request)
        {
            if (string.IsNullOrEmpty(request.ConfirmationToken))
                return BadRequest(new { message = "Confirmation token is required." });

            // Decode and verify the token matches the changes
            List<MenuChangeRequest>? tokenChanges;
            try
            {
                var decoded = System.Text.Encoding.UTF8.GetString(
                    Convert.FromBase64String(request.ConfirmationToken));
                tokenChanges = JsonSerializer.Deserialize<List<MenuChangeRequest>>(decoded);
            }
            catch
            {
                return BadRequest(new { message = "Invalid confirmation token." });
            }

            if (tokenChanges == null)
                return BadRequest(new { message = "Invalid confirmation token." });

            // Apply each change
            foreach (var change in request.Changes)
            {
                if (change.Action == "Add" && change.NewItem != null)
                {
                    var item = new MenuItem
                    {
                        DayOfWeek = change.NewItem.DayOfWeek,
                        MealType = change.NewItem.MealType,
                        ItemName = change.NewItem.ItemName,
                        Description = change.NewItem.Description,
                        PhotoUrl = change.NewItem.PhotoUrl,
                        DisplayOrder = change.NewItem.DisplayOrder
                    };
                    _context.MenuItems.Add(item);
                }
                else if (change.Action == "Update" && change.MenuItemID != null)
                {
                    var existing = _context.MenuItems
                        .FirstOrDefault(m => m.MenuItemID == change.MenuItemID);

                    if (existing != null && change.UpdatedItem != null)
                    {
                        if (change.UpdatedItem.ItemName != null)
                            existing.ItemName = change.UpdatedItem.ItemName;
                        if (change.UpdatedItem.Description != null)
                            existing.Description = change.UpdatedItem.Description;
                        if (change.UpdatedItem.PhotoUrl != null)
                            existing.PhotoUrl = change.UpdatedItem.PhotoUrl;
                        if (change.UpdatedItem.DisplayOrder != null)
                            existing.DisplayOrder = change.UpdatedItem.DisplayOrder.Value;
                    }
                }
                else if (change.Action == "Delete" && change.MenuItemID != null)
                {
                    var existing = _context.MenuItems
                        .FirstOrDefault(m => m.MenuItemID == change.MenuItemID);

                    if (existing != null)
                        _context.MenuItems.Remove(existing);
                }
            }

            _context.SaveChanges();

            return Ok(new { message = $"{request.Changes.Count} menu change(s) applied successfully." });
        }
    }
}