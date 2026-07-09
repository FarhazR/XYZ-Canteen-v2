namespace CanteenAPI.DTOs
{
    public class CreateUserRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string UserType { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = "Employee";
        public string Password { get; set; } = string.Empty;
    }

    public class UpdateRoleRequest
    {
        public string Role { get; set; } = string.Empty;
    }
}