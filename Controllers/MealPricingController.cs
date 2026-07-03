using CanteenAPI.Data;
using CanteenAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CanteenAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MealPricingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MealPricingController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/MealPricing
        [HttpGet]
        [Authorize]
        public IActionResult GetPricing()
        {
            var pricing = _context.MealPricing
                .Select(p => new MealPricingResponse
                {
                    MealType = p.MealType,
                    BaseCost = p.BaseCost,
                    PaneerSurcharge = p.PaneerSurcharge,
                    NonVegSurcharge = p.NonVegSurcharge
                })
                .ToList();

            return Ok(pricing);
        }

        // PUT /api/MealPricing
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdatePricing([FromBody] List<UpdateMealPricingRequest> updates)
        {
            var validMealTypes = new[] { "Breakfast", "Lunch", "Evening Snacks", "Dinner" };

            foreach (var update in updates)
            {
                if (!validMealTypes.Contains(update.MealType))
                    return BadRequest(new { message = $"Invalid meal type: {update.MealType}" });

                if (update.BaseCost < 0 || update.PaneerSurcharge < 0 || update.NonVegSurcharge < 0)
                    return BadRequest(new { message = "Costs cannot be negative." });

                var existing = _context.MealPricing
                    .FirstOrDefault(p => p.MealType == update.MealType);

                if (existing != null)
                {
                    existing.BaseCost = update.BaseCost;
                    existing.PaneerSurcharge = update.PaneerSurcharge;
                    existing.NonVegSurcharge = update.NonVegSurcharge;
                }
            }

            _context.SaveChanges();
            return Ok(new { message = "Pricing updated successfully." });
        }
    }
}