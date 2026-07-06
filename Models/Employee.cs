using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CanteenAPI.Models
{
    [Table("Employee")]
    public class Employee
    {
        [Key]
        public int ID { get; set; }
        public string? Empno { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Designation { get; set; }
        public string? Department { get; set; }
        public string? Grade { get; set; }
        public string? Location { get; set; }
        public string? Unit { get; set; }
        public string? Email { get; set; }
        public string? MobileNo { get; set; }
        public string? Intercom { get; set; }
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string? title { get; set; }
        public int empState { get; set; }
        public int isConnectLock { get; set; }
        public DateTime? connectLockFrom { get; set; }
    }
}