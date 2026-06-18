using CanteenAPI.Data;
using CanteenAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CanteenAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MenuController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MenuController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/menu
        [HttpGet]
        public IActionResult GetAll()
        {
            var days = new List<string>
            {
                "Monday", "Tuesday", "Wednesday",
                "Thursday", "Friday", "Saturday", "Sunday"
            };

            var mealTypes = new List<string>
            {
                "Breakfast", "Lunch", "Evening Snacks", "Dinner"
            };

            var items = _context.MenuItems
                .OrderBy(m => m.DisplayOrder)
                .Select(m => new MenuItemResponse
                {
                    MenuItemID = m.MenuItemID,
                    DayOfWeek = m.DayOfWeek,
                    MealType = m.MealType,
                    ItemName = m.ItemName,
                    Description = m.Description,
                    PhotoUrl = m.PhotoUrl,
                    DisplayOrder = m.DisplayOrder
                })
                .ToList();

            // Group by day then by meal type for easy frontend consumption
            var grouped = days.Select(day => new
            {
                Day = day,
                Meals = mealTypes.Select(mealType => new
                {
                    MealType = mealType,
                    Items = items
                        .Where(i => i.DayOfWeek == day && i.MealType == mealType)
                        .ToList()
                }).ToList()
            }).ToList();

            return Ok(grouped);
        }

        // GET api/menu/{day}
        [HttpGet("{day}")]
        public IActionResult GetByDay(string day)
        {
            var validDays = new List<string>
            {
                "Monday", "Tuesday", "Wednesday",
                "Thursday", "Friday", "Saturday", "Sunday"
            };

            if (!validDays.Contains(day))
                return BadRequest(new { message = "Invalid day. Use full day name e.g. Monday." });

            var items = _context.MenuItems
                .Where(m => m.DayOfWeek == day)
                .OrderBy(m => m.DisplayOrder)
                .Select(m => new MenuItemResponse
                {
                    MenuItemID = m.MenuItemID,
                    DayOfWeek = m.DayOfWeek,
                    MealType = m.MealType,
                    ItemName = m.ItemName,
                    Description = m.Description,
                    PhotoUrl = m.PhotoUrl,
                    DisplayOrder = m.DisplayOrder
                })
                .ToList();

            if (!items.Any())
                return NotFound(new { message = $"No menu items found for {day}." });

            return Ok(items);
        }
    }
}
