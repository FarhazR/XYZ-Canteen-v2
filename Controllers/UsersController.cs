using CanteenAPI.Data;
using CanteenAPI.DTOs;
using CanteenAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            // Validate required fields
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "Name is required." });

            if (string.IsNullOrWhiteSpace(request.Username))
                return BadRequest(new { message = "Username is required." });

            if (string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Password is required." });

            if (string.IsNullOrWhiteSpace(request.Department))
                return BadRequest(new { message = "Department is required." });

            if (string.IsNullOrWhiteSpace(request.Phone))
                return BadRequest(new { message = "Phone is required." });

            // Validate role
            if (request.Role != "Employee" && request.Role != "Admin")
                return BadRequest(new { message = "Role must be either 'Employee' or 'Admin'." });

            // Enforce username uniqueness (case-insensitive)
            var usernameExists = _context.Employees
                .Any(e => e.Username == request.Username.ToLower().Trim());

            if (usernameExists)
                return Conflict(new { message = "Username already exists." });

            var employee = new Employee
            {
                Name = request.Name,
                Username = request.Username.ToLower().Trim(),
                Department = request.Department,
                Phone = request.Phone,
                Role = request.Role,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _context.Employees.Add(employee);
            _context.SaveChanges();

            return Ok(new
            {
                message = "User created successfully.",
                employeeID = employee.EmployeeID
            });
        }
    }
}