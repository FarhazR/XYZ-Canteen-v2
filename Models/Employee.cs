namespace CanteenAPI.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set;} = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = "Employee";
        public string PasswordHash { get; set; } = string.Empty;
    }
}
