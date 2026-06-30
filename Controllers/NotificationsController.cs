using CanteenAPI.Data;
using CanteenAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CanteenAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/Notifications/my
        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMy()
        {
            var employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (employeeIdClaim == null) return Unauthorized();
            var employeeId = int.Parse(employeeIdClaim);

            var notifications = await _context.Notifications
                .Where(n => n.EmployeeID == employeeId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationResponseDto
                {
                    NotificationID = n.NotificationID,
                    Title = n.Title,
                    Message = n.Message,
                    RelatedBookingID = n.RelatedBookingID,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();

            return Ok(notifications);
        }

        // PUT /api/Notifications/{id}/read
        [HttpPut("{id}/read")]
        [Authorize]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (employeeIdClaim == null) return Unauthorized();
            var employeeId = int.Parse(employeeIdClaim);

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationID == id && n.EmployeeID == employeeId);

            if (notification == null)
                return NotFound(new { message = "Notification not found." });

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Notification marked as read." });
        }

        // PUT /api/Notifications/mark-all-read
        [HttpPut("mark-all-read")]
        [Authorize]
        public async Task<IActionResult> MarkAllRead()
        {
            var employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (employeeIdClaim == null) return Unauthorized();
            var employeeId = int.Parse(employeeIdClaim);

            var unread = await _context.Notifications
                .Where(n => n.EmployeeID == employeeId && !n.IsRead)
                .ToListAsync();

            unread.ForEach(n => n.IsRead = true);
            await _context.SaveChangesAsync();

            return Ok(new { message = "All notifications marked as read." });
        }
    }
}