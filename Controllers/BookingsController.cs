using CanteenAPI.Data;
using CanteenAPI.DTOs;
using CanteenAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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
            "Central Canteen", "Administrative Building", "Central Control Room", "Central Workshop"
        };

        private decimal CalculateTotalCost(Booking booking, List<MealPricing> pricingList)
        {
            var pricing = pricingList.FirstOrDefault(p => p.MealType == booking.MealType);

            if (pricing == null) return 0;

            var vegCost = booking.VegCount * pricing.BaseCost;
            var paneerCost = booking.PaneerCount * (pricing.BaseCost + pricing.PaneerSurcharge);
            var nonVegCost = booking.NonVegCount * (pricing.BaseCost + pricing.NonVegSurcharge);

            // Breakfast and Evening Snacks have no veg/paneer/nonveg counts
            // so all three will be 0 — fall back to base cost × headcount
            var headCount = booking.BookingFor == "Guests" ? (booking.GuestCount ?? 1) : 1;
            var subtotal = vegCost + paneerCost + nonVegCost;

            var mealCost = subtotal > 0 ? subtotal : pricing.BaseCost * headCount;

            var addOnCost = booking.AddOns?.Sum(a => a.TotalCost) ?? 0;

            return mealCost + addOnCost;
        }
        
        public BookingsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserID()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        private string GetCurrentUserType()
        {
            return User.FindFirst("UserType")?.Value ?? "Employee";
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
            var pricingList = _context.MealPricing.ToList();

            var bookings = _context.Bookings
                .Include(b => b.NewUser)
                .Include(b => b.AddOns).ThenInclude(ba => ba.AddOn)
                .OrderByDescending(b => b.FromDate)
                .ToList();

            // Manually resolve employee names for IT employee bookings
            var employeeIDs = bookings
                .Where(b => b.EmployeeID != null)
                .Select(b => b.EmployeeID!.Value)
                .Distinct()
                .ToList();

            var employeeNames = _context.Employee
                .Where(e => employeeIDs.Contains(e.ID))
                .ToDictionary(e => e.ID, e => e.Name);

            var result = bookings.Select(b => new BookingResponse
            {
                BookingID = b.BookingID,
                EmployeeID = b.EmployeeID,
                NewUserID = b.NewUserID,
                EmployeeName = b.EmployeeID != null
                    ? (employeeNames.TryGetValue(b.EmployeeID.Value, out var eName) ? eName : string.Empty)
                    : b.NewUser != null ? b.NewUser.Name : string.Empty,
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
                IsCollected = b.IsCollected,
                CollectedAt = b.CollectedAt,
                TotalCost = CalculateTotalCost(b, pricingList),
                BookingGroupID = b.BookingGroupID,
                AddOns = b.AddOns.Select(ba => new BookingAddOnResponse
                {
                    AddOnID = ba.AddOnID,
                    Name = ba.AddOn.Name,
                    Quantity = ba.Quantity,
                    CostPerUnit = ba.CostPerUnit,
                    TotalCost = ba.TotalCost
                }).ToList(),
                CanModify = b.Status == "Confirmed" &&
                            b.FromDate.Date >= DateTime.Today &&
                            !IsPastCutoff(b.FromDate, b.MealType)
            }).ToList();

            return Ok(result);
        }

        // GET api/bookings/my
        [HttpGet("my")]
        public IActionResult GetMyBookings()
        {
            var employeeID = GetCurrentUserID();
            var userID = GetCurrentUserID();
            var userType = GetCurrentUserType();

            var pricingList = _context.MealPricing.ToList();

            var bookings = _context.Bookings
                .Include(b => b.Employee)
                .Include(b => b.NewUser)
                .Include(b => b.AddOns).ThenInclude(ba => ba.AddOn)
                .Where(b => userType == "Employee" ? b.EmployeeID == userID : b.NewUserID == userID)
                .OrderByDescending(b => b.FromDate)
                .ToList()
                .Select(b => new BookingResponse
                {
                    BookingID = b.BookingID,
                    EmployeeID = b.EmployeeID,
                    NewUserID = b.NewUserID,
                    EmployeeName = b.Employee != null ? b.Employee.Name : b.NewUser != null ? b.NewUser.Name : string.Empty,
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
                    IsCollected = b.IsCollected,
                    CollectedAt = b.CollectedAt,
                    TotalCost = CalculateTotalCost(b, pricingList),
                    BookingGroupID = b.BookingGroupID,
                    AddOns = b.AddOns.Select(ba => new BookingAddOnResponse
                    {
                        AddOnID = ba.AddOnID,
                        Name = ba.AddOn.Name,
                        Quantity = ba.Quantity,
                        CostPerUnit = ba.CostPerUnit,
                        TotalCost = ba.TotalCost
                    }).ToList(),
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
            var userID = GetCurrentUserID();
            var userType = GetCurrentUserType();

            var pricingList = _context.MealPricing.ToList();
            
            var booking = _context.Bookings
                .Include(b => b.Employee)
                .Include(b => b.NewUser)
                .Include(b => b.AddOns).ThenInclude(ba => ba.AddOn)
                .Where(b => b.BookingID == id && (isAdmin || (userType == "Employee" ? b.EmployeeID == userID : b.NewUserID == userID)))
                .FirstOrDefault();

            if (booking == null)
                return NotFound(new { message = "Booking not found." });

            return Ok(new BookingResponse
            {
                BookingID = booking.BookingID,
                EmployeeID = booking.EmployeeID,
                NewUserID = booking.NewUserID,
                EmployeeName = booking.Employee != null ? booking.Employee.Name: booking.NewUser != null ? booking.NewUser.Name : string.Empty,

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
                IsCollected = booking.IsCollected,
                CollectedAt = booking.CollectedAt,
                TotalCost = CalculateTotalCost(booking, pricingList),
                BookingGroupID = booking.BookingGroupID,
                AddOns = booking.AddOns.Select(ba => new BookingAddOnResponse
                {
                    AddOnID = ba.AddOnID,
                    Name = ba.AddOn.Name,
                    Quantity = ba.Quantity,
                    CostPerUnit = ba.CostPerUnit,
                    TotalCost = ba.TotalCost
                }).ToList(),
                CanModify = booking.Status == "Confirmed" &&
                            booking.FromDate.Date >= DateTime.Today &&
                            !IsPastCutoff(booking.FromDate, booking.MealType)
                
            });
        }

        // POST api/bookings
        [HttpPost]
        public IActionResult Create([FromBody] CreateBookingRequest request)
        {
            var userID = GetCurrentUserID();
            var userType = GetCurrentUserType();

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

            // Validate meals list
            if (request.Meals == null || request.Meals.Count == 0)
                return BadRequest(new { message = "At least one meal must be selected." });

            // Validate no duplicate meal types in request
            var mealTypes = request.Meals.Select(m => m.MealType).ToList();
            if (mealTypes.Count != mealTypes.Distinct().Count())
                return BadRequest(new { message = "Duplicate meal types in request." });

            // Validate guest booking
            if (request.BookingFor == "Guests" &&
                (request.GuestCount == null || request.GuestCount <= 0))
                return BadRequest(new { message = "Guest count is required for guest bookings." });

            // Validate each meal
            var errors = new List<string>();

            foreach (var meal in request.Meals)
            {
                if (!ValidMealTypes.Contains(meal.MealType))
                {
                    errors.Add($"{meal.MealType}: Invalid meal type.");
                    continue;
                }

                // Check cutoff
                if (IsPastCutoff(request.FromDate, meal.MealType))
                {
                    errors.Add($"{meal.MealType}: Cutoff time has passed.");
                    continue;
                }

                // Validate meal counts
                if (!meal.IsSpecialMeal)
                {
                    if (request.BookingFor == "Self")
                    {
                        var totalCount = meal.VegCount + meal.PaneerCount + meal.NonVegCount;
                        if (meal.MealType != "Breakfast" && meal.MealType != "Evening Snacks")
                        {
                            if (totalCount == 0)
                                errors.Add($"{meal.MealType}: Please select a meal category.");
                            else if (totalCount > 1)
                                errors.Add($"{meal.MealType}: Self booking allows only one meal.");
                        }
                    }

                    if (request.BookingFor == "Guests")
                    {
                        var totalMeals = meal.VegCount + meal.PaneerCount + meal.NonVegCount;
                        if (meal.MealType != "Breakfast" && meal.MealType != "Evening Snacks")
                        {
                            if (totalMeals == 0)
                                errors.Add($"{meal.MealType}: Please enter meal counts for guests.");
                            else if (request.GuestCount.HasValue && totalMeals != request.GuestCount.Value)
                                errors.Add($"{meal.MealType}: Total meal counts must equal number of guests.");
                        }
                    }
                }

                //Validate isSpecialMeal
                if (meal.IsSpecialMeal)
                {
                    if (meal.MealType != "Lunch" && meal.MealType != "Dinner")
                        errors.Add($"{meal.MealType}: Today's Special is only available for Lunch and Dinner.");
                    else
                    {
                        var specialExists = _context.TodaysSpecials
                            .Any(s => s.Date.Date == request.FromDate.Date &&
                                      s.MealType == meal.MealType &&
                                      s.ApplicableOutlets.Contains(request.CanteenLocation));
                        if (!specialExists)
                            errors.Add($"{meal.MealType}: No Today's Special available for the selected date and outlet.");
                    }
                }

                // Check duplicate booking for each date in range
                var currentDate = request.FromDate.Date;
                while (currentDate <= request.ToDate.Date)
                {
                    bool duplicateExists = userType == "Employee"
                        ? _context.Bookings.Any(b =>
                            b.EmployeeID == userID &&
                            b.MealType == meal.MealType &&
                            b.FromDate.Date <= currentDate &&
                            b.ToDate.Date >= currentDate &&
                            b.Status == "Confirmed")
                        : _context.Bookings.Any(b =>
                            b.NewUserID == userID &&
                            b.MealType == meal.MealType &&
                            b.FromDate.Date <= currentDate &&
                            b.ToDate.Date >= currentDate &&
                            b.Status == "Confirmed");

                    if (duplicateExists)
                    {
                        errors.Add($"{meal.MealType}: Booking already exists for {currentDate:yyyy-MM-dd}.");
                        break;
                    }
                    currentDate = currentDate.AddDays(1);
                }
            }

            if (errors.Count > 0)
                return BadRequest(new { message = "Booking failed due to the following errors.", errors });

            // All validations passed — create bookings
            var groupID = Guid.NewGuid();
            var pricingList = _context.MealPricing.ToList();
            var createdBookings = new List<int>();

            foreach (var meal in request.Meals)
            {
                var booking = new Booking
                {
                    BookingGroupID = groupID,
                    EmployeeID = userType == "Employee" ? userID : null,
                    NewUserID = userType == "NewUser" ? userID : null,
                    FromDate = request.FromDate.Date,
                    ToDate = request.ToDate.Date,
                    CanteenLocation = request.CanteenLocation,
                    MealType = meal.MealType,
                    BookingFor = request.BookingFor,
                    GuestCount = request.BookingFor == "Guests" ? request.GuestCount : null,
                    VegCount = meal.VegCount,
                    PaneerCount = meal.PaneerCount,
                    NonVegCount = meal.NonVegCount,
                    IsSpecialMeal = meal.IsSpecialMeal,
                    Status = "Confirmed",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Bookings.Add(booking);
                _context.SaveChanges();

                // Process add-ons for this booking
                if (meal.AddOns != null && meal.AddOns.Count > 0)
                {
                    foreach (var addOnRequest in meal.AddOns)
                    {
                        var addOn = _context.AddOns
                            .FirstOrDefault(a => a.AddOnID == addOnRequest.AddOnID && a.IsAvailable);

                        if (addOn == null) continue;

                        if (addOnRequest.Quantity <= 0) continue;

                        var bookingAddOn = new BookingAddOn
                        {
                            BookingID = booking.BookingID,
                            AddOnID = addOn.AddOnID,
                            Quantity = addOnRequest.Quantity,
                            CostPerUnit = addOn.CostPerUnit,
                            TotalCost = addOnRequest.Quantity * addOn.CostPerUnit
                        };
                        
                        _context.BookingAddOns.Add(bookingAddOn);
                    }
                    _context.SaveChanges();
                }

                createdBookings.Add(booking.BookingID);
            }

            return Ok(new
            {
                message = "Booking confirmed.",
                bookingGroupID = groupID,
                bookingIDs = createdBookings
            });
        }

        // PUT api/bookings/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateBookingRequest request)
        {
            var userID = GetCurrentUserID();
            var userType = GetCurrentUserType();

            var booking = _context.Bookings
                .FirstOrDefault(b => b.BookingID == id && (userType == "Employee" ? b.EmployeeID == userID : b.NewUserID == userID));

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

        // PUT api/bookings/group/{groupId}
        [HttpPut("group/{groupId}")]
        public IActionResult UpdateGroup(Guid groupId, [FromBody] UpdateBookingGroupRequest request)
        {
            var userID = GetCurrentUserID();
            var userType = GetCurrentUserType();

            // Get all confirmed bookings in this group belonging to this user
            var groupBookings = _context.Bookings
                .Where(b => b.BookingGroupID == groupId &&
                    b.Status == "Confirmed" &&
                    (userType == "Employee" ? b.EmployeeID == userID : b.NewUserID == userID))
                .ToList();

            if (!groupBookings.Any())
                return NotFound(new { message = "Booking group not found." });

            var errors = new List<string>();
            var mealsToKeep = request.Meals.Select(m => m.MealType).ToList();

            // Validate requested meal updates
            foreach (var meal in request.Meals)
            {
                if (IsPastCutoff(groupBookings.First().FromDate, meal.MealType))
                    errors.Add($"{meal.MealType}: Cutoff time has passed, cannot modify.");
            }

            // Check meals being cancelled are within cutoff
            foreach (var existing in groupBookings)
            {
                if (!mealsToKeep.Contains(existing.MealType))
                {
                    if (IsPastCutoff(existing.FromDate, existing.MealType))
                        errors.Add($"{existing.MealType}: Cutoff time has passed, cannot cancel.");
                }
            }

            if (errors.Count > 0)
                return BadRequest(new { message = "Update failed.", errors });

            // Cancel meals not in the updated list
            foreach (var existing in groupBookings)
            {
                if (!mealsToKeep.Contains(existing.MealType))
                    existing.Status = "Cancelled";
            }

            // Update meals that are in the updated list
            foreach (var meal in request.Meals)
            {
                var existing = groupBookings.FirstOrDefault(b => b.MealType == meal.MealType);
                if (existing != null)
                {
                    
                    //Validate isSpecialMeal
                    if (meal.IsSpecialMeal)
                    {
                        if (meal.MealType != "Lunch" && meal.MealType != "Dinner")
                            return BadRequest(new { message = $"{meal.MealType}: Today's Special is only available for Lunch and Dinner."});
                
                        var specialExists = _context.TodaysSpecials
                            .Any(s => s.Date.Date == existing.FromDate.Date &&
                                      s.MealType == meal.MealType &&
                                      s.ApplicableOutlets.Contains(existing.CanteenLocation));
                        if (!specialExists)
                            return BadRequest(new { message = $"{meal.MealType}: No Today's Special available for the selected date and outlet."});
                
                }
                    existing.VegCount = meal.VegCount;
                    existing.PaneerCount = meal.PaneerCount;
                    existing.NonVegCount = meal.NonVegCount;
                    existing.IsSpecialMeal = meal.IsSpecialMeal;

                    // Update add-ons
                    if (meal.AddOns != null)
                    {
                        // Remove existing add-ons for this booking
                        var existingAddOns = _context.BookingAddOns
                            .Where(ba => ba.BookingID == existing.BookingID)
                            .ToList();
                        _context.BookingAddOns.RemoveRange(existingAddOns);

                        // Add updated add-ons
                        foreach (var addOnRequest in meal.AddOns)
                        {
                            var addOn = _context.AddOns
                                .FirstOrDefault(a => a.AddOnID == addOnRequest.AddOnID && a.IsAvailable);

                            if (addOn == null) continue;
                            if (addOnRequest.Quantity <= 0) continue;

                            _context.BookingAddOns.Add(new BookingAddOn
                            {
                                BookingID = existing.BookingID,
                                AddOnID = addOn.AddOnID,
                                Quantity = addOnRequest.Quantity,
                                CostPerUnit = addOn.CostPerUnit,
                                TotalCost = addOnRequest.Quantity * addOn.CostPerUnit
                            });
                        }
                        _context.SaveChanges();
                    }

                }
            }

            _context.SaveChanges();

            return Ok(new { message = "Booking group updated successfully." });
        }

        // DELETE api/bookings/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userID = GetCurrentUserID();
            var userType = GetCurrentUserType();

            var booking = _context.Bookings
                .FirstOrDefault(b => b.BookingID == id);

            if (booking == null)
                return NotFound(new { message = "Booking not found." });

            var isAdmin = User.IsInRole("Admin");
            var isOwnBooking = userType == "Employee"
                ? booking.EmployeeID == userID
                : booking.NewUserID == userID;

            if (!isAdmin && !isOwnBooking)
                return NotFound(new { message = "Booking not found." });

            if (booking.Status == "Cancelled")
                return BadRequest(new { message = "Booking is already cancelled." });

            if (IsPastCutoff(booking.FromDate, booking.MealType))
                return BadRequest(new { message = "Cutoff time has passed. Cannot cancel this booking." });

            booking.Status = "Cancelled";
            _context.SaveChanges();

            if (isAdmin && !isOwnBooking)
            {
                var notif = new Notification
                {
                    EmployeeID = booking.EmployeeID,
                    NewUserID = booking.NewUserID,
                    Title = "Booking Cancelled",
                    Message = $"Your booking #{booking.BookingID} for {booking.MealType} on {booking.FromDate:yyyy-MM-dd} has been cancelled by the admin.",
                    RelatedBookingID = booking.BookingID,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Notifications.Add(notif);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Booking cancelled successfully." });
        }

        // PUT /api/Bookings/{id}/collect
        [HttpPut("{id}/collect")]
        [Authorize(Roles = "Admin")]
        public IActionResult Collect(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingID == id);

            if (booking == null)
                return NotFound(new { message = "Booking not found." });

            if (booking.Status == "Cancelled")
                return BadRequest(new { message = "Cannot mark a cancelled booking as collected." });

            booking.IsCollected = true;
            booking.CollectedAt = DateTime.UtcNow;
            _context.SaveChanges();

            return Ok(new { message = "Marked as collected." });
        }

        // PUT /api/Bookings/{id}/uncollect
        [HttpPut("{id}/uncollect")]
        [Authorize(Roles = "Admin")]
        public IActionResult Uncollect(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingID == id);

            if (booking == null)
                return NotFound(new { message = "Booking not found." });

            booking.IsCollected = false;
            booking.CollectedAt = null;
            _context.SaveChanges();

            return Ok(new { message = "Marked as not collected." });
        }
    }
}