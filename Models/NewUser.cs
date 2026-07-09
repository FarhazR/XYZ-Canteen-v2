namespace CanteenAPI.Models
{
    public class NewUser
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string UserType { get; set; } = string.Empty; // Contractual Employee, Apprentice, Intern, Guest
        public string Phone { get; set; } = string.Empty; // Unique identifier
        public string Role { get; set; } = "Employee"; // Employee or Admin
        public string PasswordHash { get; set; } = string.Empty;

        // Audit fields
        public int? CreatedByEmployeeID { get; set; }
        public int? CreatedByNewUserID { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Lock fields
        public bool IsLocked { get; set; } = false;
        public DateTime? LockedAt { get; set; }
        public int? LockedByEmployeeID { get; set; }
        public int? LockedByNewUserID { get; set; }

        // Navigation properties
        //public Employee? CreatedByEmployee { get; set; }
        public NewUser? CreatedByNewUser { get; set; }
        //public Employee? LockedByEmployee { get; set; }
        public NewUser? LockedByNewUser { get; set; }
    }
}