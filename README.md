```markdown
# Canteen Meal Booking System — Backend

ASP.NET Core Web API backend for the PSU Canteen Meal Booking mobile application.

---

## Technology Stack

| Component | Technology | Version |
|-----------|------------|---------|
| Runtime | .NET | 8.0 |
| Framework | ASP.NET Core Web API | 8.0 |
| ORM | Entity Framework Core | 8.0.0 |
| Database (Prototype) | SQLite | 8.0.0 |
| Authentication | JWT Bearer | 8.0.0 |
| Password Hashing | BCrypt.Net-Next | 4.0.3 |
| API Documentation | Swashbuckle (Swagger) | 6.5.0 |

---

## Prerequisites

- .NET 8 SDK — download from https://dotnet.microsoft.com/download
- dotnet-ef CLI tool

Verify .NET installation:
```
dotnet --version
```
Should show 8.x.x

Install EF Core CLI tool (one-time setup):
```
dotnet tool install --global dotnet-ef --version 8.0.0
```

---

## Getting Started

### 1. Clone the repository
```
git clone [repository url]
cd CanteenAPI
```

### 2. Restore packages
```
dotnet restore
```

### 3. Create and seed the database
```
dotnet ef database update
```

This creates a local `canteen.db` SQLite file and automatically seeds it with
dummy employees and the weekly menu on first run.

### 4. Run the project
```
dotnet run --launch-profile http
```

The API will be available at `http://0.0.0.0:[port]`
Swagger UI available at `http://localhost:[port]/swagger`

The port number is shown in the terminal on startup.

---

## Making the API accessible on a local network (for mobile testing)

By default the API listens on all interfaces (0.0.0.0) when using the http
launch profile. To access it from another device on the same WiFi network:

1. Find your machine's IPv4 address:
```
ipconfig
```
Look for IPv4 Address under your active network adapter.

2. Allow the port through Windows Firewall:
```
netsh advfirewall firewall add rule name="CanteenAPI" dir=in action=allow protocol=TCP localport=[port]
```

3. The frontend can then reach the API at:
```
http://[your-ipv4]:[port]/api
```

---

## Test Credentials

### Admin Account
- Employee ID: 1
- Password: admin123
- Name: Rajesh Kumar
- Department: Engineering

### Employee Account 1
- Employee ID: 2
- Password: employee123
- Name: Priya Sharma
- Department: HR

### Employee Account 2
- Employee ID: 3
- Password: employee123
- Name: Amit Das
- Department: Finance

---

## API Endpoints

All endpoints except `/api/auth/login` require a valid JWT token in the
Authorization header:
```
Authorization: Bearer [token]
```

### Authentication
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| POST | /api/auth/login | Public | Login, returns JWT token |
| GET | /api/auth/me | Employee, Admin | Get current user info |

### Menu
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| GET | /api/menu | Employee, Admin | Full weekly menu grouped by day |
| GET | /api/menu/{day} | Employee, Admin | Single day menu (e.g. Monday) |

### Bookings
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| POST | /api/bookings | Employee, Admin | Place a new booking |
| GET | /api/bookings/my | Employee, Admin | Get current user's bookings |
| GET | /api/bookings/{id} | Employee, Admin | Get a specific booking |
| PUT | /api/bookings/{id} | Employee, Admin | Modify a booking |
| DELETE | /api/bookings/{id} | Employee, Admin | Cancel a booking |

### Today's Special
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| GET | /api/specials/today | Employee, Admin | Get today's active specials |
| GET | /api/specials | Employee, Admin | Get all published specials |
| POST | /api/specials | Admin only | Publish a new special |
| PUT | /api/specials/{id} | Admin only | Update a published special |
| DELETE | /api/specials/{id} | Admin only | Remove a published special |

---

## Valid Field Values

### Meal Types
- Breakfast
- Lunch
- Evening Snacks
- Dinner

### Canteen Locations (placeholders — to be updated with actual outlet names)
- Outlet 1
- Outlet 2
- Outlet 3
- Outlet 4

### Booking For
- Self
- Guests

### Today's Special Meal Types (Lunch and Dinner only)
- Lunch
- Dinner

---

## Cutoff Times

| Meal Type | Cutoff Time |
|-----------|-------------|
| Breakfast | 8:30 AM |
| Lunch | 10:30 AM |
| Evening Snacks | 3:00 PM |
| Dinner | 5:00 PM |

Bookings, modifications, and cancellations are not permitted after the
cutoff time for that meal on the booking's From Date.

---

## Sample Requests

### Login
```
POST /api/auth/login
Content-Type: application/json

{
  "employeeID": 1,
  "password": "admin123"
}
```

### Place a Booking (Self)
```
POST /api/bookings
Authorization: Bearer [token]
Content-Type: application/json

{
  "fromDate": "2026-06-16",
  "toDate": "2026-06-16",
  "canteenLocation": "Outlet 1",
  "mealType": "Lunch",
  "bookingFor": "Self",
  "vegCount": 1,
  "paneerCount": 0,
  "nonVegCount": 0,
  "isSpecialMeal": false
}
```

### Place a Booking (Guests)
```
POST /api/bookings
Authorization: Bearer [token]
Content-Type: application/json

{
  "fromDate": "2026-06-16",
  "toDate": "2026-06-18",
  "canteenLocation": "Outlet 2",
  "mealType": "Dinner",
  "bookingFor": "Guests",
  "guestCount": 5,
  "vegCount": 3,
  "paneerCount": 1,
  "nonVegCount": 1,
  "isSpecialMeal": false
}
```

### Publish Today's Special (Admin only)
```
POST /api/specials
Authorization: Bearer [admin token]
Content-Type: application/json

{
  "specialName": "Mutton Biryani",
  "description": "Slow cooked mutton biryani with raita and salan",
  "photoUrl": null,
  "mealType": "Lunch",
  "date": "2026-06-16",
  "applicableOutlets": ["Outlet 1", "Outlet 2"]
}
```

---

## Project Structure

```
CanteenAPI/
├── Controllers/
│   ├── AuthController.cs
│   ├── BookingsController.cs
│   ├── MenuController.cs
│   └── SpecialsController.cs
├── Data/
│   ├── AppDbContext.cs
│   └── DbSeeder.cs
├── DTOs/
│   ├── AuthDTOs.cs
│   ├── BookingDTOs.cs
│   ├── MenuDTOs.cs
│   └── SpecialDTOs.cs
├── migrations/
├── Models/
│   ├── Booking.cs
│   ├── Employee.cs
│   ├── MenuItem.cs
│   └── TodaysSpecial.cs
├── Properties/
│   └── launchSettings.json
├── appsettings.json
├── canteen.db
├── CanteenAPI.csproj
└── Program.cs
```

---

## Notes for Frontend Integration

- The `canModify` field in booking responses indicates whether the booking
  can still be edited or cancelled. The frontend should use this flag to
  show or hide edit and cancel buttons rather than calculating cutoff
  times independently.
- Today's Special outlets are returned as a list of strings. During booking,
  check if the selected outlet appears in the special's `applicableOutlets`
  list before showing the special option to the user.
- The weekly menu is fixed and does not change. It can be cached on the
  frontend after the first load.

---

## Known Limitations (Prototype)

- Self booking does not yet enforce single meal category selection on the
  backend — to be added before production
- Outlet names are placeholders pending confirmation from canteen management
- Breakfast cutoff time (8:30 AM) is subject to change
- No file upload support for photos — URL references only
- Production SQL Server integration pending

---

*Developed as part of PSU internship project, June 2026.*
```

---