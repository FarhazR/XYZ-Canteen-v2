using CanteenAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CanteenAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<TodaysSpecial> TodaysSpecials { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Notification> Notifications { get; set; }
    }
}
