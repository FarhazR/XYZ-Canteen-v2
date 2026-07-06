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
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // POST /api/Users
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateUser([FromBody] CreateUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "Name is required." });

            if (string.IsNullOrWhiteSpace(request.Username))
                return BadRequest(new { message = "Username is required." });

            if (string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Password is required." });

            if (string.IsNullOrWhiteSpace(request.Phone))
                return BadRequest(new { message = "Phone is required." });

            if (string.IsNullOrWhiteSpace(request.UserType))
                return BadRequest(new { message = "UserType is required." });

            var validUserTypes = new[] { "Contractual Employee", "Apprentice", "Intern", "Guest" };
            if (!validUserTypes.Contains(request.UserType))
                return BadRequest(new { message = "UserType must be one of: Contractual Employee, Apprentice, Intern, Guest." });

            if (request.Role != "Employee" && request.Role != "Admin")
                return BadRequest(new { message = "Role must be either 'Employee' or 'Admin'." });

            // Check username uniqueness across both tables
            var usernameExistsInEmployees = _context.Employees
                .Any(e => e.username == request.Username.ToLower().Trim());

            var usernameExistsInNewUsers = _context.NewUsers
                .Any(u => u.Username == request.Username.ToLower().Trim());

            if (usernameExistsInEmployees || usernameExistsInNewUsers)
                return Conflict(new { message = "Username already exists." });

            // Check phone uniqueness in NewUsers
            var phoneExists = _context.NewUsers
                .Any(u => u.Phone == request.Phone.Trim());

            if (phoneExists)
                return Conflict(new { message = "A user with this phone number already exists." });

            // Get creator's ID and UserType from JWT
            var creatorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var creatorUserType = User.FindFirst("UserType")?.Value;

            var newUser = new NewUser
            {
                Name = request.Name,
                Username = request.Username.ToLower().Trim(),
                Department = request.Department,
                UserType = request.UserType,
                Phone = request.Phone.Trim(),
                Role = request.Role,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                CreatedByEmployeeID = creatorUserType == "Employee" ? creatorId : null,
                CreatedByNewUserID = creatorUserType == "NewUser" ? creatorId : null
            };

            _context.NewUsers.Add(newUser);
            _context.SaveChanges();

            return Ok(new
            {
                message = "User created successfully.",
                userID = newUser.ID
            });
        }

        // PUT /api/Users/{id}/role
        [HttpPut("{id}/role")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateRole(int id, [FromBody] UpdateRoleRequest request)
        {
            if (request.Role != "Employee" && request.Role != "Admin")
                return BadRequest(new { message = "Role must be either 'Employee' or 'Admin'." });

            var newUser = _context.NewUsers.FirstOrDefault(u => u.ID == id);

            if (newUser == null)
                return NotFound(new { message = "User not found." });

            newUser.Role = request.Role;
            _context.SaveChanges();

            return Ok(new { message = "Role updated successfully." });
        }

        // PUT /api/Users/{id}/lock
        [HttpPut("{id}/lock")]
        [Authorize(Roles = "Admin")]
        public IActionResult LockUser(int id)
        {
            var newUser = _context.NewUsers.FirstOrDefault(u => u.ID == id);

            if (newUser == null)
                return NotFound(new { message = "User not found." });

            if (newUser.IsLocked)
                return BadRequest(new { message = "User is already locked." });

            var lockerID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var lockerUserType = User.FindFirst("UserType")?.Value;

            newUser.IsLocked = true;
            newUser.LockedAt = DateTime.UtcNow;
            newUser.LockedByEmployeeID = lockerUserType == "Employee" ? lockerID : null;
            newUser.LockedByNewUserID = lockerUserType == "NewUser" ? lockerID : null;

            _context.SaveChanges();

            return Ok(new { message = "User locked successfully." });
        }
    }
}