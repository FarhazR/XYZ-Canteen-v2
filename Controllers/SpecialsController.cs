using CanteenAPI.Data;
using CanteenAPI.DTOs;
using CanteenAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CanteenAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SpecialsController : ControllerBase
    {
        private readonly AppDbContext _context;

        private static readonly List<string> ValidMealTypes = new()
        {
            "Lunch", "Dinner"
        };

        private static readonly List<string> ValidLocations = new()
        {
            "Outlet 1", "Outlet 2", "Outlet 3", "Outlet 4"
        };

        public SpecialsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetCurrentEmployeeID()
        {
            return int.Parse(User
                .FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        // GET api/specials/today
        [HttpGet("today")]
        public IActionResult GetToday()
        {
            var today = DateTime.Today;

            var specials = _context.TodaysSpecials
                .Where(s => s.Date.Date == today)
                .ToList()
                .Select(s => new SpecialResponse
                {
                    SpecialID = s.SpecialID,
                    SpecialName = s.SpecialName,
                    Description = s.Description,
                    PhotoUrl = s.PhotoUrl,
                    MealType = s.MealType,
                    Date = s.Date,
                    ApplicableOutlets = s.ApplicableOutlets
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(o => o.Trim())
                        .ToList(),
                    PublishedBy = s.CreatedBy != null
                        ? s.CreatedBy.Name
                        : string.Empty,
                    CreatedAt = s.CreatedAt
                })
                .ToList();

            return Ok(specials);
        }

        // GET api/specials
        [HttpGet]
        public IActionResult GetAll()
        {
            var specials = _context.TodaysSpecials
                .OrderByDescending(s => s.Date)
                .ToList()
                .Select(s => new SpecialResponse
                {
                    SpecialID = s.SpecialID,
                    SpecialName = s.SpecialName,
                    Description = s.Description,
                    PhotoUrl = s.PhotoUrl,
                    MealType = s.MealType,
                    Date = s.Date,
                    ApplicableOutlets = s.ApplicableOutlets
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(o => o.Trim())
                        .ToList(),
                    PublishedBy = s.CreatedBy != null
                        ? s.CreatedBy.Name
                        : string.Empty,
                    CreatedAt = s.CreatedAt
                })
                .ToList();

            return Ok(specials);
        }

        // POST api/specials — admin only
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] CreateSpecialRequest request)
        {
            var employeeID = GetCurrentEmployeeID();

            if (!ValidMealTypes.Contains(request.MealType))
                return BadRequest(new { message = "Today's Special is only available for Lunch and Dinner." });

            if (request.ApplicableOutlets == null || !request.ApplicableOutlets.Any())
                return BadRequest(new { message = "At least one outlet must be selected." });

            var invalidOutlets = request.ApplicableOutlets
                .Where(o => !ValidLocations.Contains(o))
                .ToList();

            if (invalidOutlets.Any())
                return BadRequest(new { message = $"Invalid outlet(s): {string.Join(", ", invalidOutlets)}" });

            if (request.Date.Date < DateTime.Today)
                return BadRequest(new { message = "Cannot publish a special for a past date." });

            var special = new TodaysSpecial
            {
                SpecialName = request.SpecialName,
                Description = request.Description,
                PhotoUrl = request.PhotoUrl,
                MealType = request.MealType,
                Date = request.Date.Date,
                ApplicableOutlets = string.Join(", ", request.ApplicableOutlets),
                CreatedByEmployeeID = employeeID,
                CreatedAt = DateTime.UtcNow
            };

            _context.TodaysSpecials.Add(special);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetToday), new
            {
                message = "Today's Special published successfully.",
                specialID = special.SpecialID
            });
        }

        // PUT api/specials/{id} — admin only
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] UpdateSpecialRequest request)
        {
            var special = _context.TodaysSpecials
                .FirstOrDefault(s => s.SpecialID == id);

            if (special == null)
                return NotFound(new { message = "Special not found." });

            if (special.Date.Date < DateTime.Today)
                return BadRequest(new { message = "Cannot edit a special for a past date." });

            if (request.MealType != null && !ValidMealTypes.Contains(request.MealType))
                return BadRequest(new { message = "Today's Special is only available for Lunch and Dinner." });

            if (request.ApplicableOutlets != null)
            {
                var invalidOutlets = request.ApplicableOutlets
                    .Where(o => !ValidLocations.Contains(o))
                    .ToList();

                if (invalidOutlets.Any())
                    return BadRequest(new { message = $"Invalid outlet(s): {string.Join(", ", invalidOutlets)}" });

                special.ApplicableOutlets = string.Join(", ", request.ApplicableOutlets);
            }

            if (request.SpecialName != null) special.SpecialName = request.SpecialName;
            if (request.Description != null) special.Description = request.Description;
            if (request.PhotoUrl != null) special.PhotoUrl = request.PhotoUrl;
            if (request.MealType != null) special.MealType = request.MealType;
            if (request.Date != null) special.Date = request.Date.Value.Date;

            _context.SaveChanges();

            return Ok(new { message = "Special updated successfully." });
        }

        // DELETE api/specials/{id} — admin only
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var special = _context.TodaysSpecials
                .FirstOrDefault(s => s.SpecialID == id);

            if (special == null)
                return NotFound(new { message = "Special not found." });

            if (special.Date.Date < DateTime.Today)
                return BadRequest(new { message = "Cannot delete a special for a past date." });

            _context.TodaysSpecials.Remove(special);
            _context.SaveChanges();

            return Ok(new { message = "Special removed successfully." });
        }
    }
}
