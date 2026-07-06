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
            if (string.IsNullOrEmpty(request.Username))
                return BadRequest(new { message = "Please provide a username." });

            var username = request.Username.ToLower().Trim();

            // Check IT's Employee table first
            var employee = _context.Employees
                .FirstOrDefault(e => e.username == username);

            if (employee != null)
            {
                // Enforce access restrictions
                if (employee.empState != 3)
                    return Unauthorized(new { message = "Account is not active." });

                if (employee.isConnectLock == 1)
                    return Unauthorized(new { message = "Account is locked." });

                if (!BCrypt.Net.BCrypt.Verify(request.Password, employee.password))
                    return Unauthorized(new { message = "Invalid credentials." });

                var token = GenerateTokenForEmployee(employee);

                return Ok(new LoginResponse
                {
                    Token = token,
                    EmployeeID = employee.ID,
                    Name = employee.Name,
                    Username = employee.username,
                    Department = employee.Department ?? string.Empty,
                    Role = "Employee",
                    UserType = "Employee"
                });
            }

            // Check NewUsers table
            var newUser = _context.NewUsers
                .FirstOrDefault(u => u.Username == username);

            if (newUser != null)
            {
                if (newUser.IsLocked)
                    return Unauthorized(new { message = "Account is locked." });

                if (!BCrypt.Net.BCrypt.Verify(request.Password, newUser.PasswordHash))
                    return Unauthorized(new { message = "Invalid credentials." });

                var token = GenerateTokenForNewUser(newUser);

                return Ok(new LoginResponse
                {
                    Token = token,
                    EmployeeID = newUser.ID,
                    Name = newUser.Name,
                    Username = newUser.Username,
                    Department = newUser.Department ?? string.Empty,
                    Role = newUser.Role,
                    UserType = "NewUser"
                });
            }

            return Unauthorized(new { message = "Invalid credentials." });
        }

        // GET api/auth/me
        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userTypeClaim = User.FindFirst("UserType")?.Value;

            if (idClaim == null || userTypeClaim == null)
                return Unauthorized();

            var id = int.Parse(idClaim);

            if (userTypeClaim == "Employee")
            {
                var employee = _context.Employees.FirstOrDefault(e => e.ID == id);
                if (employee == null) return NotFound();

                return Ok(new LoginResponse
                {
                    EmployeeID = employee.ID,
                    Name = employee.Name,
                    Username = employee.username,
                    Department = employee.Department ?? string.Empty,
                    Role = "Employee",
                    UserType = "Employee"
                });
            }
            else
            {
                var newUser = _context.NewUsers.FirstOrDefault(u => u.ID == id);
                if (newUser == null) return NotFound();

                return Ok(new LoginResponse
                {
                    EmployeeID = newUser.ID,
                    Name = newUser.Name,
                    Username = newUser.Username,
                    Department = newUser.Department ?? string.Empty,
                    Role = newUser.Role,
                    UserType = "NewUser"
                });
            }
        }

        private string GenerateTokenForEmployee(CanteenAPI.Models.Employee employee)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, employee.ID.ToString()),
                new Claim(ClaimTypes.Name, employee.Name),
                new Claim(ClaimTypes.Role, "Employee"),
                new Claim("UserType", "Employee")
            };
            return BuildToken(claims);
        }

        private string GenerateTokenForNewUser(CanteenAPI.Models.NewUser newUser)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, newUser.ID.ToString()),
                new Claim(ClaimTypes.Name, newUser.Name),
                new Claim(ClaimTypes.Role, newUser.Role),
                new Claim("UserType", "NewUser")
            };
            return BuildToken(claims);
        }

        private string BuildToken(Claim[] claims)
        {
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