using CanteenAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CanteenAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employee { get; set; }
        public DbSet<NewUser> NewUsers { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<TodaysSpecial> TodaysSpecials { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<MealPricing> MealPricing { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map Employee to IT's existing table name and exclude from migrations
            modelBuilder.Entity<Employee>().ToTable("Employee", t => t.ExcludeFromMigrations());

            // Configure NewUser self-referencing relationships explicitly
            modelBuilder.Entity<NewUser>()
                .HasOne(u => u.CreatedByNewUser)
                .WithMany()
                .HasForeignKey(u => u.CreatedByNewUserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NewUser>()
                .HasOne(u => u.LockedByNewUser)
                .WithMany()
                .HasForeignKey(u => u.LockedByNewUserID)
                .OnDelete(DeleteBehavior.Restrict);

            /* Configure NewUser → Employee relationships
            modelBuilder.Entity<NewUser>()
                .HasOne(u => u.CreatedByEmployee)
                .WithMany()
                .HasForeignKey(u => u.CreatedByEmployeeID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NewUser>()
                .HasOne(u => u.LockedByEmployee)
                .WithMany()
                .HasForeignKey(u => u.LockedByEmployeeID)
                .OnDelete(DeleteBehavior.Restrict); */

            // Don't create FK constraints to Employee table for Bookings and Notifications
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Employee)
                .WithMany()
                .HasForeignKey(b => b.EmployeeID)
                .HasConstraintName("FK_Bookings_Employee_EmployeeID")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Employee)
                .WithMany()
                .HasForeignKey(n => n.EmployeeID)
                .HasConstraintName("FK_Notifications_Employee_EmployeeID")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}