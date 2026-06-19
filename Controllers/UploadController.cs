using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CanteenAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UploadController : ControllerBase
    {
        private static readonly List<string> AllowedExtensions = new()
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        private readonly IWebHostEnvironment _env;

        public UploadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // POST api/upload/photo
        [HttpPost("photo")]
        public async Task<IActionResult> UploadPhoto(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file was uploaded." });

            if (file.Length > MaxFileSizeBytes)
                return BadRequest(new { message = "File size must not exceed 5 MB." });

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
                return BadRequest(new { message = "Only JPG, PNG, and WEBP files are allowed." });

            var uploadsFolder = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUrl = $"/uploads/{uniqueFileName}";

            return Ok(new { photoUrl = fileUrl });
        }
    }
}