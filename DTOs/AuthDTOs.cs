namespace CanteenAPI.DTOs
{
    public class LoginRequest
    {
        //public int? EmployeeID { get; set; }
        public string? Username { get; set; }
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public int EmployeeID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
