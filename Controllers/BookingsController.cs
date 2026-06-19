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
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        private static readonly Dictionary<string, TimeSpan> CutoffTimes = new()
        {
            { "Breakfast", new TimeSpan(8, 30, 0) },
            { "Lunch", new TimeSpan(10, 30, 0) },
            { "Evening Snacks", new TimeSpan(15, 0, 0) },
            { "Dinner", new TimeSpan(17, 0, 0) }
        };

        private static readonly List<string> ValidMealTypes = new()
        {
            "Breakfast", "Lunch", "Evening Snacks", "Dinner"
        };

        private static readonly List<string> ValidLocations = new()
        {
            "Central Canteen", "Administrative Buillding", "Central Control Room", "Central Workshop"
        };

        public BookingsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetCurrentEmployeeID()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        private static bool IsPastCutoff(DateTime date, string mealType)
        {
            if (!CutoffTimes.ContainsKey(mealType)) return false;
            var cutoff = date.Date + CutoffTimes[mealType];
            return DateTime.Now > cutoff;
        }

        // GET api/bookings — admin only
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            var bookings = _context.Bookings
                .OrderByDescending(b => b.FromDate)
                .ToList()
                .Select(b => new BookingResponse
                {
                    BookingID = b.BookingID,
                    EmployeeID = b.EmployeeID,
                    EmployeeName = b.Employee != null ? b.Employee.Name : string.Empty,
                    FromDate = b.FromDate,
                    ToDate = b.ToDate,
                    CanteenLocation = b.CanteenLocation,
                    MealType = b.MealType,
                    BookingFor = b.BookingFor,
                    GuestCount = b.GuestCount,
                    VegCount = b.VegCount,
                    PaneerCount = b.PaneerCount,
                    NonVegCount = b.NonVegCount,
                    IsSpecialMeal = b.IsSpecialMeal,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt,
                    CanModify = b.Status == "Confirmed" &&
                                b.FromDate.Date >= DateTime.Today &&
                                !IsPastCutoff(b.FromDate, b.MealType)
                })
                .ToList();

            return Ok(bookings);
        }

        // GET api/bookings/my
        [HttpGet("my")]
        public IActionResult GetMyBookings()
        {
            var employeeID = GetCurrentEmployeeID();

            var bookings = _context.Bookings
                .Where(b => b.EmployeeID == employeeID)
                .OrderByDescending(b => b.FromDate)
                .ToList()
                .Select(b => new BookingResponse
                {
                    BookingID = b.BookingID,
                    EmployeeID = b.EmployeeID,
                    EmployeeName = b.Employee != null ? b.Employee.Name : string.Empty,
                    FromDate = b.FromDate,
                    ToDate = b.ToDate,
                    CanteenLocation = b.CanteenLocation,
                    MealType = b.MealType,
                    BookingFor = b.BookingFor,
                    GuestCount = b.GuestCount,
                    VegCount = b.VegCount,
                    PaneerCount = b.PaneerCount,
                    NonVegCount = b.NonVegCount,
                    IsSpecialMeal = b.IsSpecialMeal,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt,
                    CanModify = b.Status == "Confirmed" &&
                                b.FromDate.Date >= DateTime.Today &&
                                !IsPastCutoff(b.FromDate, b.MealType)
                })
                .ToList();

            return Ok(bookings);
        }

        // GET api/bookings/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var employeeID = GetCurrentEmployeeID();

            var booking = _context.Bookings
                .Where(b => b.BookingID == id && b.EmployeeID == employeeID)
                .FirstOrDefault();

            if (booking == null)
                return NotFound(new { message = "Booking not found." });

            return Ok(new BookingResponse
            {
                BookingID = booking.BookingID,
                EmployeeID = booking.EmployeeID,
                EmployeeName = booking.Employee != null ? booking.Employee.Name : string.Empty,
                FromDate = booking.FromDate,
                ToDate = booking.ToDate,
                CanteenLocation = booking.CanteenLocation,
                MealType = booking.MealType,
                BookingFor = booking.BookingFor,
                GuestCount = booking.GuestCount,
                VegCount = booking.VegCount,
                PaneerCount = booking.PaneerCount,
                NonVegCount = booking.NonVegCount,
                IsSpecialMeal = booking.IsSpecialMeal,
                Status = booking.Status,
                CreatedAt = booking.CreatedAt,
                CanModify = booking.Status == "Confirmed" &&
                            booking.FromDate.Date >= DateTime.Today &&
                            !IsPastCutoff(booking.FromDate, booking.MealType)
            });
        }

        // POST api/bookings
        [HttpPost]
        public IActionResult Create([FromBody] CreateBookingRequest request)
        {
            var employeeID = GetCurrentEmployeeID();

            // Validate meal type
            if (!ValidMealTypes.Contains(request.MealType))
                return BadRequest(new { message = "Invalid meal type." });

            // Validate location
            if (!ValidLocations.Contains(request.CanteenLocation))
                return BadRequest(new { message = "Invalid canteen location." });

            // Validate dates
            if (request.FromDate.Date < DateTime.Today)
                return BadRequest(new { message = "From date cannot be in the past." });

            if (request.ToDate.Date < request.FromDate.Date)
                return BadRequest(new { message = "To date cannot be before From date." });

            if (request.FromDate.Date > DateTime.Today.AddDays(30))
                return BadRequest(new { message = "Cannot book more than 30 days in advance." });

            // Check cutoff
            if (IsPastCutoff(request.FromDate, request.MealType))
                return BadRequest(new { message = $"Booking cutoff time for {request.MealType} has passed." });

            // Validate guest booking
            if (request.BookingFor == "Guests" &&
                (request.GuestCount == null || request.GuestCount <= 0))
                return BadRequest(new { message = "Guest count is required for guest bookings." });

            // Validate meal counts
            if (!request.IsSpecialMeal)
            {
                if (request.BookingFor == "Self")
                {
                    var totalCount = request.VegCount + request.PaneerCount + request.NonVegCount;
                    if (totalCount == 0)
                        return BadRequest(new { message = "Please select a meal category." });
                    if (totalCount > 1)
                        return BadRequest(new { message = "Self booking allows only one meal." });
                }

                if (request.BookingFor == "Guests")
                {
                    var totalMeals = request.VegCount + request.PaneerCount + request.NonVegCount;
                    if (totalMeals == 0)
                        return BadRequest(new { message = "Please enter meal counts for guests." });
                    if (request.GuestCount.HasValue && totalMeals != request.GuestCount.Value)
                        return BadRequest(new { message = "Total meal counts must equal number of guests." });
                }
            }

            // Validate Today's Special if selected
            if (request.IsSpecialMeal)
            {
                if (request.MealType != "Lunch" && request.MealType != "Dinner")
                    return BadRequest(new { message = "Today's Special is only available for Lunch and Dinner." });

                var specialExists = _context.TodaysSpecials
                    .Any(s => s.Date.Date == request.FromDate.Date &&
                              s.MealType == request.MealType &&
                              s.ApplicableOutlets.Contains(request.CanteenLocation));

                if (!specialExists)
                    return BadRequest(new { message = "No Today's Special is available for the selected date, meal type, and outlet." });
            }

            var booking = new Booking
            {
                EmployeeID = employeeID,
                FromDate = request.FromDate.Date,
                ToDate = request.ToDate.Date,
                CanteenLocation = request.CanteenLocation,
                MealType = request.MealType,
                BookingFor = request.BookingFor,
                GuestCount = request.BookingFor == "Guests" ? request.GuestCount : null,
                VegCount = request.VegCount,
                PaneerCount = request.PaneerCount,
                NonVegCount = request.NonVegCount,
                IsSpecialMeal = request.IsSpecialMeal,
                Status = "Confirmed",
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = booking.BookingID },
                new { message = "Booking confirmed.", bookingID = booking.BookingID });
        }

        // PUT api/bookings/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateBookingRequest request)
        {
            var employeeID = GetCurrentEmployeeID();

            var booking = _context.Bookings
                .FirstOrDefault(b => b.BookingID == id && b.EmployeeID == employeeID);

            if (booking == null)
                return NotFound(new { message = "Booking not found." });

            if (booking.Status == "Cancelled")
                return BadRequest(new { message = "Cannot modify a cancelled booking." });

            if (IsPastCutoff(booking.FromDate, booking.MealType))
                return BadRequest(new { message = "Cutoff time for this booking has passed." });

            if (request.MealType != null && !ValidMealTypes.Contains(request.MealType))
                return BadRequest(new { message = "Invalid meal type." });

            if (request.CanteenLocation != null && !ValidLocations.Contains(request.CanteenLocation))
                return BadRequest(new { message = "Invalid canteen location." });

            if (request.FromDate != null) booking.FromDate = request.FromDate.Value.Date;
            if (request.ToDate != null) booking.ToDate = request.ToDate.Value.Date;
            if (request.CanteenLocation != null) booking.CanteenLocation = request.CanteenLocation;
            if (request.MealType != null) booking.MealType = request.MealType;
            if (request.BookingFor != null) booking.BookingFor = request.BookingFor;
            if (request.GuestCount != null) booking.GuestCount = request.GuestCount;
            if (request.VegCount != null) booking.VegCount = request.VegCount.Value;
            if (request.PaneerCount != null) booking.PaneerCount = request.PaneerCount.Value;
            if (request.NonVegCount != null) booking.NonVegCount = request.NonVegCount.Value;
            if (request.IsSpecialMeal != null) booking.IsSpecialMeal = request.IsSpecialMeal.Value;

            _context.SaveChanges();

            return Ok(new { message = "Booking updated successfully.", bookingID = booking.BookingID });
        }

        // DELETE api/bookings/{id}
        [HttpDelete("{id}")]
        public IActionResult Cancel(int id)
        {
            var employeeID = GetCurrentEmployeeID();

            var booking = _context.Bookings
                .FirstOrDefault(b => b.BookingID == id && b.EmployeeID == employeeID);

            if (booking == null)
                return NotFound(new { message = "Booking not found." });

            if (booking.Status == "Cancelled")
                return BadRequest(new { message = "Booking is already cancelled." });

            if (IsPastCutoff(booking.FromDate, booking.MealType))
                return BadRequest(new { message = "Cutoff time has passed. Cannot cancel this booking." });

            booking.Status = "Cancelled";
            _context.SaveChanges();

            return Ok(new { message = "Booking cancelled successfully." });
        }
    }
}