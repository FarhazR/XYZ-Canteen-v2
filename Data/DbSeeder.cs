using CanteenAPI.Models;

namespace CanteenAPI.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            // Employee seeding removed — IT's Employee table owns that data
            
            // Seed default admin into NewUsers if none exists
            if (!context.NewUsers.Any(u => u.Role == "Admin"))
            {
                var adminUser = new NewUser
                {
                    Name = "Farhaz Rahman",
                    Username = "farhaz.rahman",
                    Department = "Engineering",
                    UserType = "Intern",
                    Phone = "9876543210",
                    Role = "Admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    CreatedAt = DateTime.UtcNow,
                    IsLocked = false
                };

                context.NewUsers.Add(adminUser);
                context.SaveChanges();
            }

            // Weekly Menu Items
            var menuItems = new List<MenuItem>
            {
                // Monday
                new MenuItem { DayOfWeek = "Monday", MealType = "Breakfast", ItemName = "Idli", Description = "Steamed rice cakes", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Monday", MealType = "Breakfast", ItemName = "Sambar", Description = "Lentil based vegetable stew", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Monday", MealType = "Breakfast", ItemName = "Coconut Chutney", Description = "Fresh coconut chutney", PhotoUrl = null, DisplayOrder = 3 },
                new MenuItem { DayOfWeek = "Monday", MealType = "Lunch", ItemName = "Steamed Rice", Description = "Plain steamed rice", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Monday", MealType = "Lunch", ItemName = "Dal", Description = "Cooked lentils", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Monday", MealType = "Lunch", ItemName = "Seasonal Vegetable Stir Fry", Description = "Stir fried seasonal vegetables", PhotoUrl = null, DisplayOrder = 3 },
                new MenuItem { DayOfWeek = "Monday", MealType = "Lunch", ItemName = "Salad", Description = "Fresh garden salad", PhotoUrl = null, DisplayOrder = 4 },
                new MenuItem { DayOfWeek = "Monday", MealType = "Lunch", ItemName = "Papad", Description = "Crispy papad", PhotoUrl = null, DisplayOrder = 5 },
                new MenuItem { DayOfWeek = "Monday", MealType = "Evening Snacks", ItemName = "Samosa", Description = "Crispy fried samosa", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Monday", MealType = "Evening Snacks", ItemName = "Tea", Description = "Hot masala tea", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Monday", MealType = "Dinner", ItemName = "Roti", Description = "Whole wheat flatbread", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Monday", MealType = "Dinner", ItemName = "Dal", Description = "Cooked lentils", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Monday", MealType = "Dinner", ItemName = "Sabzi", Description = "Seasonal vegetable curry", PhotoUrl = null, DisplayOrder = 3 },
                new MenuItem { DayOfWeek = "Monday", MealType = "Dinner", ItemName = "Rice", Description = "Steamed rice", PhotoUrl = null, DisplayOrder = 4 },

                // Tuesday
                new MenuItem { DayOfWeek = "Tuesday", MealType = "Breakfast", ItemName = "Poha", Description = "Flattened rice cooked with vegetables and spices", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Tuesday", MealType = "Breakfast", ItemName = "Jalebi", Description = "Sweet crispy jalebi", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Tuesday", MealType = "Lunch", ItemName = "Steamed Rice", Description = "Plain steamed rice", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Tuesday", MealType = "Lunch", ItemName = "Chana Dal", Description = "Cooked chana dal", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Tuesday", MealType = "Lunch", ItemName = "Seasonal Vegetable Stir Fry", Description = "Stir fried seasonal vegetables", PhotoUrl = null, DisplayOrder = 3 },
                new MenuItem { DayOfWeek = "Tuesday", MealType = "Lunch", ItemName = "Salad", Description = "Fresh garden salad", PhotoUrl = null, DisplayOrder = 4 },
                new MenuItem { DayOfWeek = "Tuesday", MealType = "Lunch", ItemName = "Papad", Description = "Crispy papad", PhotoUrl = null, DisplayOrder = 5 },
                new MenuItem { DayOfWeek = "Tuesday", MealType = "Evening Snacks", ItemName = "Bread Pakoda", Description = "Fried bread pakoda", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Tuesday", MealType = "Evening Snacks", ItemName = "Tea", Description = "Hot masala tea", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Tuesday", MealType = "Dinner", ItemName = "Roti", Description = "Whole wheat flatbread", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Tuesday", MealType = "Dinner", ItemName = "Rajma", Description = "Kidney bean curry", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Tuesday", MealType = "Dinner", ItemName = "Rice", Description = "Steamed rice", PhotoUrl = null, DisplayOrder = 3 },
                new MenuItem { DayOfWeek = "Tuesday", MealType = "Dinner", ItemName = "Salad", Description = "Fresh garden salad", PhotoUrl = null, DisplayOrder = 4 },

                // Wednesday
                new MenuItem { DayOfWeek = "Wednesday", MealType = "Breakfast", ItemName = "Upma", Description = "Semolina cooked with vegetables and spices", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Wednesday", MealType = "Breakfast", ItemName = "Coconut Chutney", Description = "Fresh coconut chutney", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Wednesday", MealType = "Lunch", ItemName = "Steamed Rice", Description = "Plain steamed rice", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Wednesday", MealType = "Lunch", ItemName = "Dal Fry", Description = "Tempered dal fry", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Wednesday", MealType = "Lunch", ItemName = "Seasonal Vegetable Stir Fry", Description = "Stir fried seasonal vegetables", PhotoUrl = null, DisplayOrder = 3 },
                new MenuItem { DayOfWeek = "Wednesday", MealType = "Lunch", ItemName = "Salad", Description = "Fresh garden salad", PhotoUrl = null, DisplayOrder = 4 },
                new MenuItem { DayOfWeek = "Wednesday", MealType = "Lunch", ItemName = "Papad", Description = "Crispy papad", PhotoUrl = null, DisplayOrder = 5 },
                new MenuItem { DayOfWeek = "Wednesday", MealType = "Evening Snacks", ItemName = "Veg Sandwich", Description = "Toasted vegetable sandwich", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Wednesday", MealType = "Evening Snacks", ItemName = "Tea", Description = "Hot masala tea", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Wednesday", MealType = "Dinner", ItemName = "Roti", Description = "Whole wheat flatbread", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Wednesday", MealType = "Dinner", ItemName = "Chole", Description = "Spiced chickpea curry", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Wednesday", MealType = "Dinner", ItemName = "Rice", Description = "Steamed rice", PhotoUrl = null, DisplayOrder = 3 },
                new MenuItem { DayOfWeek = "Wednesday", MealType = "Dinner", ItemName = "Raita", Description = "Yogurt with vegetables", PhotoUrl = null, DisplayOrder = 4 },

                // Thursday
                new MenuItem { DayOfWeek = "Thursday", MealType = "Breakfast", ItemName = "Puri", Description = "Fried whole wheat bread", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Thursday", MealType = "Breakfast", ItemName = "Aloo Sabzi", Description = "Spiced potato curry", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Thursday", MealType = "Lunch", ItemName = "Steamed Rice", Description = "Plain steamed rice", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Thursday", MealType = "Lunch", ItemName = "Moong Dal", Description = "Cooked moong dal", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Thursday", MealType = "Lunch", ItemName = "Seasonal Vegetable Stir Fry", Description = "Stir fried seasonal vegetables", PhotoUrl = null, DisplayOrder = 3 },
                new MenuItem { DayOfWeek = "Thursday", MealType = "Lunch", ItemName = "Salad", Description = "Fresh garden salad", PhotoUrl = null, DisplayOrder = 4 },
                new MenuItem { DayOfWeek = "Thursday", MealType = "Lunch", ItemName = "Papad", Description = "Crispy papad", PhotoUrl = null, DisplayOrder = 5 },
                new MenuItem { DayOfWeek = "Thursday", MealType = "Evening Snacks", ItemName = "Dhokla", Description = "Steamed fermented dhokla", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Thursday", MealType = "Evening Snacks", ItemName = "Tea", Description = "Hot masala tea", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Thursday", MealType = "Dinner", ItemName = "Roti", Description = "Whole wheat flatbread", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Thursday", MealType = "Dinner", ItemName = "Dal Makhani", Description = "Slow cooked black lentils", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Thursday", MealType = "Dinner", ItemName = "Rice", Description = "Steamed rice", PhotoUrl = null, DisplayOrder = 3 },
                new MenuItem { DayOfWeek = "Thursday", MealType = "Dinner", ItemName = "Salad", Description = "Fresh garden salad", PhotoUrl = null, DisplayOrder = 4 },

                // Friday
                new MenuItem { DayOfWeek = "Friday", MealType = "Breakfast", ItemName = "Dosa", Description = "Crispy rice and lentil crepe", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Friday", MealType = "Breakfast", ItemName = "Sambar", Description = "Lentil based vegetable stew", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Friday", MealType = "Breakfast", ItemName = "Coconut Chutney", Description = "Fresh coconut chutney", PhotoUrl = null, DisplayOrder = 3 },
                new MenuItem { DayOfWeek = "Friday", MealType = "Lunch", ItemName = "Steamed Rice", Description = "Plain steamed rice", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Friday", MealType = "Lunch", ItemName = "Arhar Dal", Description = "Cooked arhar dal", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Friday", MealType = "Lunch", ItemName = "Seasonal Vegetable Stir Fry", Description = "Stir fried seasonal vegetables", PhotoUrl = null, DisplayOrder = 3 },
                new MenuItem { DayOfWeek = "Friday", MealType = "Lunch", ItemName = "Salad", Description = "Fresh garden salad", PhotoUrl = null, DisplayOrder = 4 },
                new MenuItem { DayOfWeek = "Friday", MealType = "Lunch", ItemName = "Papad", Description = "Crispy papad", PhotoUrl = null, DisplayOrder = 5 },
                new MenuItem { DayOfWeek = "Friday", MealType = "Evening Snacks", ItemName = "Kachori", Description = "Fried stuffed pastry", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Friday", MealType = "Evening Snacks", ItemName = "Tea", Description = "Hot masala tea", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Friday", MealType = "Dinner", ItemName = "Roti", Description = "Whole wheat flatbread", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Friday", MealType = "Dinner", ItemName = "Paneer Curry", Description = "Cottage cheese in spiced gravy", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Friday", MealType = "Dinner", ItemName = "Rice", Description = "Steamed rice", PhotoUrl = null, DisplayOrder = 3 },
                new MenuItem { DayOfWeek = "Friday", MealType = "Dinner", ItemName = "Salad", Description = "Fresh garden salad", PhotoUrl = null, DisplayOrder = 4 },

                // Saturday
                new MenuItem { DayOfWeek = "Saturday", MealType = "Breakfast", ItemName = "Aloo Paratha", Description = "Stuffed potato flatbread", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Saturday", MealType = "Breakfast", ItemName = "Curd", Description = "Fresh yogurt", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Saturday", MealType = "Breakfast", ItemName = "Pickle", Description = "Mixed pickle", PhotoUrl = null, DisplayOrder = 3 },
                new MenuItem { DayOfWeek = "Saturday", MealType = "Lunch", ItemName = "Steamed Rice", Description = "Plain steamed rice", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Saturday", MealType = "Lunch", ItemName = "Dal", Description = "Cooked lentils", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Saturday", MealType = "Lunch", ItemName = "Mix Veg", Description = "Mixed vegetable curry", PhotoUrl = null, DisplayOrder = 3 },
                new MenuItem { DayOfWeek = "Saturday", MealType = "Lunch", ItemName = "Salad", Description = "Fresh garden salad", PhotoUrl = null, DisplayOrder = 4 },
                new MenuItem { DayOfWeek = "Saturday", MealType = "Lunch", ItemName = "Papad", Description = "Crispy papad", PhotoUrl = null, DisplayOrder = 5 },
                new MenuItem { DayOfWeek = "Saturday", MealType = "Evening Snacks", ItemName = "Poha", Description = "Flattened rice snack", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Saturday", MealType = "Evening Snacks", ItemName = "Tea", Description = "Hot masala tea", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Saturday", MealType = "Dinner", ItemName = "Roti", Description = "Whole wheat flatbread", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Saturday", MealType = "Dinner", ItemName = "Kadhi", Description = "Yogurt based curry", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Saturday", MealType = "Dinner", ItemName = "Rice", Description = "Steamed rice", PhotoUrl = null, DisplayOrder = 3 },
                new MenuItem { DayOfWeek = "Saturday", MealType = "Dinner", ItemName = "Papad", Description = "Crispy papad", PhotoUrl = null, DisplayOrder = 4 },

                // Sunday
                new MenuItem { DayOfWeek = "Sunday", MealType = "Breakfast", ItemName = "Chole Bhature", Description = "Fried bread with spiced chickpeas", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Sunday", MealType = "Breakfast", ItemName = "Pickle", Description = "Mixed pickle", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Sunday", MealType = "Lunch", ItemName = "Steamed Rice", Description = "Plain steamed rice", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Sunday", MealType = "Lunch", ItemName = "Dal", Description = "Cooked lentils", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Sunday", MealType = "Lunch", ItemName = "Seasonal Vegetable Stir Fry", Description = "Stir fried seasonal vegetables", PhotoUrl = null, DisplayOrder = 3 },
                new MenuItem { DayOfWeek = "Sunday", MealType = "Lunch", ItemName = "Sweet Dish", Description = "Dessert of the day", PhotoUrl = null, DisplayOrder = 4 },
                new MenuItem { DayOfWeek = "Sunday", MealType = "Lunch", ItemName = "Salad", Description = "Fresh garden salad", PhotoUrl = null, DisplayOrder = 5 },
                new MenuItem { DayOfWeek = "Sunday", MealType = "Lunch", ItemName = "Papad", Description = "Crispy papad", PhotoUrl = null, DisplayOrder = 6 },
                new MenuItem { DayOfWeek = "Sunday", MealType = "Evening Snacks", ItemName = "Pakoda", Description = "Mixed vegetable fritters", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Sunday", MealType = "Evening Snacks", ItemName = "Tea", Description = "Hot masala tea", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Sunday", MealType = "Dinner", ItemName = "Roti", Description = "Whole wheat flatbread", PhotoUrl = null, DisplayOrder = 1 },
                new MenuItem { DayOfWeek = "Sunday", MealType = "Dinner", ItemName = "Shahi Paneer", Description = "Cottage cheese in rich gravy", PhotoUrl = null, DisplayOrder = 2 },
                new MenuItem { DayOfWeek = "Sunday", MealType = "Dinner", ItemName = "Rice", Description = "Steamed rice", PhotoUrl = null, DisplayOrder = 3 },
                new MenuItem { DayOfWeek = "Sunday", MealType = "Dinner", ItemName = "Salad", Description = "Fresh garden salad", PhotoUrl = null, DisplayOrder = 4 },
            };

            context.MenuItems.AddRange(menuItems);
            context.SaveChanges();
        }
    }
}
