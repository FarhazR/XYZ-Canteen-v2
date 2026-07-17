using CanteenAPI.Data;
using CanteenAPI.DTOs;
using CanteenAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CanteenAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddOnsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AddOnsController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/AddOns
        [HttpGet]
        [Authorize]
        public IActionResult GetAll()
        {
            var addOns = _context.AddOns
                .Where(a => a.IsAvailable)
                .Select(a => new AddOnResponse
                {
                    AddOnID = a.AddOnID,
                    Name = a.Name,
                    CostPerUnit = a.CostPerUnit,
                    IsAvailable = a.IsAvailable
                })
                .ToList();

            return Ok(addOns);
        }

        // POST /api/AddOns
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] CreateAddOnRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "Name is required." });

            if (request.CostPerUnit < 0)
                return BadRequest(new { message = "Cost cannot be negative." });

            var addOn = new AddOn
            {
                Name = request.Name,
                CostPerUnit = request.CostPerUnit,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.AddOns.Add(addOn);
            _context.SaveChanges();

            return Ok(new { message = "Add-on created successfully.", addOnID = addOn.AddOnID });
        }

        // PUT /api/AddOns/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] UpdateAddOnRequest request)
        {
            var addOn = _context.AddOns.FirstOrDefault(a => a.AddOnID == id);

            if (addOn == null)
                return NotFound(new { message = "Add-on not found." });

            if (request.CostPerUnit.HasValue && request.CostPerUnit < 0)
                return BadRequest(new { message = "Cost cannot be negative." });

            if (request.Name != null) addOn.Name = request.Name;
            if (request.CostPerUnit.HasValue) addOn.CostPerUnit = request.CostPerUnit.Value;
            if (request.IsAvailable.HasValue) addOn.IsAvailable = request.IsAvailable.Value;

            _context.SaveChanges();

            return Ok(new { message = "Add-on updated successfully." });
        }
    }
}