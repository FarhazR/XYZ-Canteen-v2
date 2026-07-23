using CanteenAPI.Data;
using CanteenAPI.DTOs;
using CanteenAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CanteenAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnnouncementsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnnouncementsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateAnnouncementDto dto)
        {
            var publishedBy = User.Identity?.Name ?? "Admin";

            DateTime now = DateTime.Now;

            var announcement = new Announcement
            {
                Title = dto.Title,
                Message = dto.Message,
                PublishedBy = publishedBy,
                CreatedAt = now,
            };

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Announcement posted successfully.",
                announcementID = announcement.AnnouncementID
            });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var announcements = await _context.Announcements
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new AnnouncementResponseDto
                {
                    AnnouncementID = a.AnnouncementID,
                    Title = a.Title,
                    Message = a.Message,
                    PublishedBy = a.PublishedBy,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return Ok(announcements);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);

            if (announcement == null)
                return NotFound(new { message = "Announcement not found." });

            _context.Announcements.Remove(announcement);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Announcement deleted successfully." });
        }
    }
}