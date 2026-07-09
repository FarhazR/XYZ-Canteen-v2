This backend application has been developed exclusively for BCPL Dibrugarh's Canteen Management System and is intended only for official use within BCPL.

# Canteen Meal Booking System — Backend

ASP.NET Core Web API backend for the PSU Canteen Meal Booking mobile application.

---

## Technology Stack

| Component | Technology | Version |
|-----------|------------|---------|
| Runtime | .NET | 8.0 |
| Framework | ASP.NET Core Web API | 8.0 |
| ORM | Entity Framework Core | 8.0.0 |
| Database | Microsoft SQL Server (Express for development) | 2022 |
| Authentication | JWT Bearer | 8.0.0 |
| Password Hashing | BCrypt.Net-Next | 4.0.3 |
| API Documentation | Swashbuckle (Swagger) | 6.5.0 |
| Database Tooling | SQL Server Management Studio (SSMS) | Latest |

---

## Prerequisites

- .NET 8 SDK — https://dotnet.microsoft.com/download
- SQL Server Express (or access to a SQL Server instance) — https://www.microsoft.com/en-us/sql-server/sql-server-downloads
- SQL Server Management Studio (recommended for inspecting the database) — https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms
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

### 3. Update the connection string

Open `appsettings.json` and confirm the `DefaultConnection` string matches
your local SQL Server instance name. Default format:
```
Server=localhost\SQLEXPRESS;Database=CanteenDB;Trusted_Connection=True;TrustServerCertificate=True;
```
Replace `localhost\SQLEXPRESS` with your own instance name if different
(check via SSMS or `services.msc` under SQL Server services).

### 4. Create and seed the database
```
dotnet ef database update
```

This creates the `CanteenDB` database on your SQL Server instance and
automatically seeds it with dummy employees and the weekly menu on first run.

### 5. Run the project
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

### Menu Administration
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| POST | /api/menu/admin/preview | Admin only | Preview Add/Update/Delete changes |
| POST | /api/menu/admin/confirm | Admin only | Apply previewed changes |

### Bookings
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| POST | /api/bookings | Employee, Admin | Place a new booking |
| GET | /api/bookings | Admin only | View all bookings |
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

### File Upload
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| POST | /api/upload/photo | Admin only | Upload a photo, returns relative URL |

---

## Valid Field Values

### Meal Types
- Breakfast
- Lunch
- Evening Snacks
- Dinner

### Canteen Locations
- Central Canteen
- Administrative Building
- Central Control Room
- Central Workshop

### Booking For
- Self
- Guests

### Today's Special Meal Types (Lunch and Dinner only)
- Lunch
- Dinner

### Menu Change Actions (admin)
- Add
- Update
- Delete

---

## Cutoff Times

| Meal Type | Cutoff Time |
|-----------|-------------|
| Breakfast | 8:30 AM |
| Lunch | 10:30 AM |
| Evening Snacks | 3:00 PM |
| Dinner | 5:00 PM |

Bookings, modifications, and cancellations are not permitted after the
cutoff time for that meal on the booking's From Date. The `canModify`
field in booking responses reflects this automatically.

---

## Photo Uploads

Photos for Today's Special and Menu Items are uploaded directly through
the app (camera or gallery) rather than referenced by external URL.

Flow:
1. Frontend uploads the image file to `POST /api/upload/photo`
2. Backend validates (jpg/jpeg/png/webp only, 5 MB max), stores it in
   `wwwroot/uploads/`, and returns a relative path like `/uploads/xxx.jpg`
3. Frontend includes that path as `photoUrl` when creating or updating a
   Special or Menu Item
4. To display the photo, the frontend must prefix the stored path with
   the backend's base URL, e.g.
   `http://[backend-ip]:[port]/uploads/xxx.jpg`

Uploaded files are stored locally for the prototype and can be migrated
to cloud storage (e.g. Azure Blob Storage) in production without changing
the API contract — only the storage implementation inside the Upload
controller would change.

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
  "fromDate": "2026-06-22",
  "toDate": "2026-06-22",
  "canteenLocation": "Central Canteen",
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
  "fromDate": "2026-06-22",
  "toDate": "2026-06-24",
  "canteenLocation": "Administrative Building",
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
  "photoUrl": "/uploads/3f2a1b4c-xxxx.jpg",
  "mealType": "Lunch",
  "date": "2026-06-22",
  "applicableOutlets": ["Central Canteen", "Administrative Building"]
}
```

### Upload a Photo (Admin only)
```
POST /api/upload/photo
Authorization: Bearer [admin token]
Content-Type: multipart/form-data

file: [binary image data]
```

### Preview Menu Changes (Admin only)
```
POST /api/menu/admin/preview
Authorization: Bearer [admin token]
Content-Type: application/json

{
  "changes": [
    {
      "action": "Add",
      "newItem": {
        "dayOfWeek": "Monday",
        "mealType": "Lunch",
        "itemName": "Buttermilk",
        "description": "Chilled spiced buttermilk",
        "photoUrl": null,
        "displayOrder": 6
      }
    }
  ]
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
│   ├── MenuAdminController.cs
│   ├── SpecialsController.cs
│   └── UploadController.cs
├── Data/
│   ├── AppDbContext.cs
│   └── DbSeeder.cs
├── DTOs/
│   ├── AuthDTOs.cs
│   ├── BookingDTOs.cs
│   ├── MenuDTOs.cs
│   └── SpecialDTOs.cs
├── Migrations/
├── Models/
│   ├── Booking.cs
│   ├── Employee.cs
│   ├── MenuItem.cs
│   └── TodaysSpecial.cs
├── Properties/
│   └── launchSettings.json
├── wwwroot/
│   └── uploads/
├── appsettings.json
├── CanteenAPI.csproj
└── Program.cs
```

---

## Notes for Frontend Integration

- The `canModify` field in booking responses indicates whether a booking
  can still be edited or cancelled. Use this flag rather than calculating
  cutoff times independently.
- Today's Special outlets are returned as a list of strings. Check if the
  selected outlet appears in the special's `applicableOutlets` before
  showing the special option during booking.
- The weekly menu rarely changes and can be cached on the frontend after
  first load.
- When displaying weekly menu items, group by `mealType` first, then sort
  by `displayOrder` within each group — items are returned sorted by
  `displayOrder` globally, not per meal type.
- Photo uploads are a two-step process: upload first via
  `/api/upload/photo`, then use the returned path in the `photoUrl` field
  of the Special or Menu item.
- Stored photo paths are relative — prefix with the backend base URL to
  render images on the frontend.

---

## Known Limitations (Prototype)

- Database currently runs on local SQL Server Express with dummy seeded
  data; production will connect to the organization's actual employee
  database
- Uploaded photos are stored on local disk; production should migrate
  this to cloud storage (e.g. Azure Blob Storage)
- `publishedBy` field on Today's Special currently returns empty —
  navigation property needs an explicit `.Include()` (cosmetic, non-blocking)

---

*Developed as part of PSU internship project, June-July 2026.*