using CanteenAPI.Models;

namespace CanteenAPI.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Employees.Any()) return;

            // Employees
            var employees = new List<Employee>
            {
                new Employee
                {
                    Name = "Rajesh Kumar",
                    Department = "Engineering",
                    Phone = "9876543210",
                    Role = "Admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123")
                },
                new Employee
                {
                    Name = "Priya Sharma",
                    Department = "HR",
                    Phone = "9876543211",
                    Role = "Employee",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("employee123")
                },
                new Employee
                {
                    Name = "Amit Das",
                    Department = "Finance",
                    Phone = "9876543212",
                    Role = "Employee",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("employee123")
                }
            };

            context.Employees.AddRange(employees);

            // Weekly Menu Items
            var menuItems = new List<MenuItem>
            {
                // Monday
                new MenuItem { DayOfWeek = "Monday", MealType = "Breakfast", ItemName = "Idli Sambar", Description = "Steamed rice cakes served with sambar and chutney", PhotoUrl = null },
                new MenuItem { DayOfWeek = "Monday", MealType = "Lunch", ItemName = "Rice, Dal, Sabzi, Salad, Papad", Description = "Steamed rice with dal, seasonal vegetable, salad and papad", PhotoUrl = null },
                new MenuItem { DayOfWeek = "Monday", MealType = "Evening Snacks", ItemName = "Samosa and Tea", Description = "Crispy samosa served with tea", PhotoUrl = null },
                new MenuItem { DayOfWeek = "Monday", MealType = "Dinner", ItemName = "Roti, Dal, Sabzi, Rice", Description = "Whole wheat roti with dal, seasonal vegetable and rice", PhotoUrl = null },

                // Tuesday
                new MenuItem { DayOfWeek = "Tuesday", MealType = "Breakfast", ItemName = "Poha", Description = "Flattened rice cooked with vegetables and spices", PhotoUrl = null },
                new MenuItem { DayOfWeek = "Tuesday", MealType = "Lunch", ItemName = "Rice, Chana Dal, Seasonal Vegetable Stir Fry, Salad, Papad", Description = "Steamed rice with chana dal, seasonal stir fry, salad and papad", PhotoUrl = null },
                new MenuItem { DayOfWeek = "Tuesday", MealType = "Evening Snacks", ItemName = "Bread Pakoda and Tea", Description = "Fried bread pakoda served with tea", PhotoUrl = null },
                new MenuItem { DayOfWeek = "Tuesday", MealType = "Dinner", ItemName = "Roti, Rajma, Rice, Salad", Description = "Whole wheat roti with rajma curry, rice and salad", PhotoUrl = null },

                // Wednesday
                new MenuItem { DayOfWeek = "Wednesday", MealType = "Breakfast", ItemName = "Upma", Description = "Semolina cooked with vegetables and spices", PhotoUrl = null },
                new MenuItem { DayOfWeek = "Wednesday", MealType = "Lunch", ItemName = "Rice, Dal Fry, Sabzi, Salad, Papad", Description = "Steamed rice with dal fry, seasonal vegetable, salad and papad", PhotoUrl = null },
                new MenuItem { DayOfWeek = "Wednesday", MealType = "Evening Snacks", ItemName = "Veg Sandwich and Tea", Description = "Toasted vegetable sandwich served with tea", PhotoUrl = null },
                new MenuItem { DayOfWeek = "Wednesday", MealType = "Dinner", ItemName = "Roti, Chole, Rice, Raita", Description = "Whole wheat roti with chole curry, rice and raita", PhotoUrl = null },

                // Thursday
                new MenuItem { DayOfWeek = "Thursday", MealType = "Breakfast", ItemName = "Puri Sabzi", Description = "Fried puri served with potato curry", PhotoUrl = null },
                new MenuItem { DayOfWeek = "Thursday", MealType = "Lunch", ItemName = "Rice, Moong Dal, Sabzi, Salad, Papad", Description = "Steamed rice with moong dal, seasonal vegetable, salad and papad", PhotoUrl = null },
                new MenuItem { DayOfWeek = "Thursday", MealType = "Evening Snacks", ItemName = "Dhokla and Tea", Description = "Steamed dhokla served with tea", PhotoUrl = null },
                new MenuItem { DayOfWeek = "Thursday", MealType = "Dinner", ItemName = "Roti, Dal Makhani, Rice, Salad", Description = "Whole wheat roti with dal makhani, rice and salad", PhotoUrl = null },

                // Friday
                new MenuItem { DayOfWeek = "Friday", MealType = "Breakfast", ItemName = "Dosa and Chutney", Description = "Crispy dosa served with coconut chutney and sambar", PhotoUrl = null },
                new MenuItem { DayOfWeek = "Friday", MealType = "Lunch", ItemName = "Rice, Arhar Dal, Sabzi, Salad, Papad", Description = "Steamed rice with arhar dal, seasonal vegetable, salad and papad", PhotoUrl = null },
                new MenuItem { DayOfWeek = "Friday", MealType = "Evening Snacks", ItemName = "Kachori and Tea", Description = "Fried kachori served with tea", PhotoUrl = null },
                new MenuItem { DayOfWeek = "Friday", MealType = "Dinner", ItemName = "Roti, Paneer Curry, Rice, Salad", Description = "Whole wheat roti with paneer curry, rice and salad", PhotoUrl = null },

                // Saturday
                new MenuItem { DayOfWeek = "Saturday", MealType = "Breakfast", ItemName = "Aloo Paratha", Description = "Stuffed potato flatbread served with curd and pickle", PhotoUrl = null },
                new MenuItem { DayOfWeek = "Saturday", MealType = "Lunch", ItemName = "Rice, Dal, Mix Veg, Salad, Papad", Description = "Steamed rice with dal, mixed vegetable curry, salad and papad", PhotoUrl = null },
                new MenuItem { DayOfWeek = "Saturday", MealType = "Evening Snacks", ItemName = "Poha and Tea", Description = "Flattened rice snack served with tea", PhotoUrl = null },
                new MenuItem { DayOfWeek = "Saturday", MealType = "Dinner", ItemName = "Roti, Kadhi, Rice, Papad", Description = "Whole wheat roti with kadhi, rice and papad", PhotoUrl = null },

                // Sunday
                new MenuItem { DayOfWeek = "Sunday", MealType = "Breakfast", ItemName = "Chole Bhature", Description = "Fried bhature served with spiced chole curry", PhotoUrl = null },
                new MenuItem { DayOfWeek = "Sunday", MealType = "Lunch", ItemName = "Rice, Dal, Sabzi, Sweet, Salad, Papad", Description = "Steamed rice with dal, seasonal vegetable, sweet dish, salad and papad", PhotoUrl = null },
                new MenuItem { DayOfWeek = "Sunday", MealType = "Evening Snacks", ItemName = "Pakoda and Tea", Description = "Mixed vegetable pakoda served with tea", PhotoUrl = null },
                new MenuItem { DayOfWeek = "Sunday", MealType = "Dinner", ItemName = "Roti, Shahi Paneer, Rice, Salad", Description = "Whole wheat roti with shahi paneer, rice and salad", PhotoUrl = null },
            };

            context.MenuItems.AddRange(menuItems);
            context.SaveChanges();
        }
    }
}
