using CanteenAPI.Data;
using CanteenAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CanteenAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var employee = _context.Employees
                .FirstOrDefault(e => e.EmployeeID == request.EmployeeID);

            if (employee == null)
                return Unauthorized(new { message = "Invalid Employee ID or password." });

            bool validPassword = BCrypt.Net.BCrypt.Verify(
                request.Password, employee.PasswordHash);

            if (!validPassword)
                return Unauthorized(new { message = "Invalid Employee ID or password." });

            var token = GenerateToken(employee);

            return Ok(new LoginResponse
            {
                Token = token,
                EmployeeID = employee.EmployeeID,
                Name = employee.Name,
                Department = employee.Department,
                Role = employee.Role
            });
        }

        // GET api/auth/me
        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var employeeIdClaim = User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (employeeIdClaim == null)
                return Unauthorized();

            var employee = _context.Employees
                .FirstOrDefault(e => e.EmployeeID == int.Parse(employeeIdClaim));

            if (employee == null)
                return NotFound();

            return Ok(new LoginResponse
            {
                EmployeeID = employee.EmployeeID,
                Name = employee.Name,
                Department = employee.Department,
                Role = employee.Role
            });
        }

        private string GenerateToken(CanteenAPI.Models.Employee employee)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, employee.EmployeeID.ToString()),
                new Claim(ClaimTypes.Name, employee.Name),
                new Claim(ClaimTypes.Role, employee.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
