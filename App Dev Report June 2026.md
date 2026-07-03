# Canteen App — App Development Report
**Date:** June 2026
**Prepared by:** Farhaz Rahman, Grihshant Manas Dutta
**Status:** Active Development — Cloud Testing Phase

---

## Technology Stack

### Backend
| Component | Technology | Version |
|-----------|------------|---------|
| Runtime | .NET | 8.0 |
| Framework | ASP.NET Core Web API | 8.0 |
| Language | C# | 12.0 (bundled with .NET 8) |
| ORM | Entity Framework Core | 8.0.0 |
| Database (Local Dev) | Microsoft SQL Server Express | 2022 |
| Database (Cloud Testing) | SQLite | 8.0.0 (via EF Core) |
| Authentication | JWT Bearer | 8.0.0 |
| Password Hashing | BCrypt.Net-Next | 4.0.3 |
| API Documentation | Swashbuckle (Swagger) | 6.5.0 |
| Database GUI | SQL Server Management Studio | Latest stable |
| Tunneling (Testing) | ngrok | 3.39.8 |

### Frontend (for reference)
| Component | Technology | Version |
|-----------|------------|---------|
| Runtime | Node.js | 24.x |
| Framework | React Native via Expo | SDK 54.0.34 |
| Language | JavaScript | ES2022 |
| Navigation | @react-navigation/native | 7.3.3 |
| Navigation Stack | @react-navigation/native-stack | 7.17.5 |
| HTTP Client | axios | 1.18.0 |
| Local Storage | @react-native-async-storage/async-storage | 2.2.0 |
| Date Picker | @react-native-community/datetimepicker | 8.4.4 |
| Image Picker | expo-image-picker | 17.0.11 |
| File System | expo-file-system | 19.0.23 |
| Safe Area | react-native-safe-area-context | 5.6.0 |
| Screens | react-native-screens | 4.16.0 |

---

## Development Timeline

### Phase 1 — Proof of Concept (Complete)
Built a minimal backend to validate React Native ↔ ASP.NET Core integration. Generic meal/order models were used as placeholders. Login, JWT auth, and basic meal endpoints were tested end to end on a physical Android device via Expo Go. This phase confirmed the technology stack was viable and identified key setup issues (version mismatches, network configuration, firewall rules).

### Phase 2 — Requirements Review (Complete)
Full requirements review session conducted based on the existing web portal's booking module. Key decisions made:
- Replaced generic meal/order model with domain-specific Booking model
- Added Today's Special feature (admin-published, Lunch/Dinner only)
- Added Weekly Menu screen with per-item display order
- Confirmed four canteen outlets (hardcoded)
- Established cutoff times per meal type
- Defined Self vs Guest booking behavior
- Produced revised SRS v2.1

### Phase 3 — Backend Rebuild (Complete)
Entire backend rebuilt from scratch with correct domain models. All controllers, DTOs, validation logic, and seeder written fresh based on revised requirements.

### Phase 4 — SQL Server Migration (Complete)
Switched from SQLite prototype database to Microsoft SQL Server Express for local development. Fresh migrations generated, data reseeded, and full regression test passed.

### Phase 5 — Cloud Testing Setup (Complete)
ngrok configured to expose local backend over HTTPS for remote frontend integration testing.

---

## Database Schema

### Employees Table
| Column | Type | Notes |
|--------|------|-------|
| EmployeeID | INT (PK, Identity) | Auto-incremented |
| Name | NVARCHAR | Full name |
| Username | NVARCHAR | Format: firstname.lastname |
| Department | NVARCHAR | |
| Phone | NVARCHAR | |
| Role | NVARCHAR | Employee or Admin |
| PasswordHash | NVARCHAR | BCrypt hashed |

### Bookings Table
| Column | Type | Notes |
|--------|------|-------|
| BookingID | INT (PK, Identity) | |
| EmployeeID | INT (FK) | References Employees |
| FromDate | DATETIME | |
| ToDate | DATETIME | |
| CanteenLocation | NVARCHAR | One of four valid outlets |
| MealType | NVARCHAR | Breakfast/Lunch/Evening Snacks/Dinner |
| BookingFor | NVARCHAR | Self or Guests |
| GuestCount | INT (Nullable) | Null if Self |
| VegCount | INT | |
| PaneerCount | INT | |
| NonVegCount | INT | |
| IsSpecialMeal | BIT | True if Today's Special selected |
| Status | NVARCHAR | Confirmed or Cancelled |
| CreatedAt | DATETIME | UTC |
| IsCollected | BIT | Default false |
| CollectedAt | DATETIME (Nullable) | UTC, set when marked as collected |

### MenuItems Table
| Column | Type | Notes |
|--------|------|-------|
| MenuItemID | INT (PK, Identity) | |
| DayOfWeek | NVARCHAR | Monday through Sunday |
| MealType | NVARCHAR | Breakfast/Lunch/Evening Snacks/Dinner |
| ItemName | NVARCHAR | |
| Description | NVARCHAR | |
| PhotoUrl | NVARCHAR (Nullable) | Relative path or null |
| DisplayOrder | INT | Preserves item order within each slot |

### MealPricing Table
| Column | Type | Notes |
| MealPricingID | INT (PK, Identity) | |
| MealType | NVARCHAR | Breakfast/Lunch/Evening Snacks/Dinner |
| BaseCost | DECIMAL | Base cost per person |
| PaneerSurcharge | DECIMAL | Additional cost for paneer |
| NonVegSurcharge | DECIMAL | Additional cost for non-veg |

### TodaysSpecials Table
| Column | Type | Notes |
|--------|------|-------|
| SpecialID | INT (PK, Identity) | |
| SpecialName | NVARCHAR | |
| Description | NVARCHAR (Nullable) | Optional |
| PhotoUrl | NVARCHAR (Nullable) | Relative path or null |
| MealType | NVARCHAR | Lunch or Dinner only |
| Date | DATETIME | Date special applies to |
| ApplicableOutlets | NVARCHAR | Comma-separated outlet names |
| CreatedByEmployeeID | INT (FK) | References Employees |
| CreatedAt | DATETIME | UTC |

### Announcements Table
| Column | Type | Notes |
|--------|------|-------|
| AnnouncementID | INT (PK, Identity) | |
| Title | NVARCHAR | |
| Message | NVARCHAR (Nullable) | Optional |
| PublishedBy | NVARCHAR | Username from JWT |
| CreatedAt | DATETIME | UTC, auto-generated|

### Notifications Table
| Column | Type | Notes |
|--------|------|-------|
| NotificationID | INT (PK, Identity) | |
| EmployeeID | INT (FK) | References Employees |
| Title | NVARCHAR | |
| Message | NVARCHAR | |
| RelatedBookingID | INT (Nullable) | References Bookings |
| IsRead | BIT | Default false |
| CreatedAt | DATETIME | UTC, auto-generated |

---

---

## Complete API Endpoint Reference

### Announcements
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| POST | /api/Announcements | Admin only | Publish a new announcement |
| GET | /api/Announcements | Authenticated | Get all announcements, most recent first |
| DELETE | /api/Announcements/{id} | Admin only | Remove an announcement |

### Auth
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| POST | /api/Auth/login | Public | Login via Username + Password, returns JWT |
| GET | /api/Auth/me | Authenticated | Get current logged-in user info |

### Bookings
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| GET | /api/Bookings | Admin only | View all bookings across all employees |
| POST | /api/Bookings | Authenticated | Place a new booking |
| GET | /api/Bookings/my | Authenticated | View own booking history |
| GET | /api/Bookings/{id} | Authenticated | View a specific booking (own only) |
| PUT | /api/Bookings/{id} | Authenticated | Modify a booking (own only, before cutoff) |
| DELETE | /api/Bookings/{id} | Authenticated | Cancel a booking (own only, before cutoff); admin can cancel any booking and triggers a notification to the employee |
| PUT | /api/Bookings/{id}/collect | Admin only | Mark a booking as collected |
| PUT | /api/Bookings/{id}/uncollect | Admin only | Undo a collection mark

### MealPricing
| Method | Endpoint | Access | Description |
| GET | /api/MealPricing | Authenticated | Get current pricing for all meal types
| PUT | /api/MealPricing | Admin only | Update pricing for one or more meal types

### Menu
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| GET | /api/Menu | Authenticated | Full weekly menu grouped by day and meal type |
| GET | /api/Menu/{day} | Authenticated | Single day's menu (e.g. /api/Menu/Monday) |

### MenuAdmin
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| POST | /api/menu/admin/preview | Admin only | Preview Add/Update/Delete changes, returns summary and confirmation token |
| POST | /api/menu/admin/confirm | Admin only | Apply previewed changes using confirmation token |

### Notifications
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| GET | /api/Notifications/my | Authenticated | Get own notifications, most recent first |
| PUT | /api/Notifications/{id}/read | Authenticated | Mark a single notification as read |
| PUT | /api/Notifications/mark-all-read | Authenticated | Mark all own notifications as read |

### Specials
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| GET | /api/Specials/today | Authenticated | Get specials active today |
| GET | /api/Specials | Authenticated | Get all published specials |
| POST | /api/Specials | Admin only | Publish a new special |
| PUT | /api/Specials/{id} | Admin only | Update a published special |
| DELETE | /api/Specials/{id} | Admin only | Remove a published special |

### Upload
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| POST | /api/Upload/photo | Admin only | Upload image (jpg/png/webp, max 5MB), returns relative URL |

### Users
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| POST | /api/UsersAdmin | onlyCreate a new user account (Employee or Admin) |

---

## Validation Rules

### Booking Creation and Modification
- FromDate cannot be in the past
- ToDate cannot be before FromDate
- Cannot book more than 30 days in advance
- Booking and modification blocked after cutoff time for that meal type on FromDate
- Self booking: exactly one meal category (Veg, Paneer, or Non-Veg) with count of 1
- Guest booking: total Veg + Paneer + Non-Veg counts must equal GuestCount
- Veg/Paneer/Non-Veg counts must be 0 for Breakfast and Evening Snacks
- IsSpecialMeal can only be true for Lunch and Dinner
- If IsSpecialMeal is true, a published special must exist for that date, meal type, and outlet

### Today's Special
- Only Lunch and Dinner are valid meal types for specials
- Cannot publish a special for a past date
- Cannot publish two specials for the same date, meal type, and outlet (conflict check per outlet)
- Cannot edit or delete a special for a past date

### File Upload
- Accepted formats: jpg, jpeg, png, webp
- Maximum file size: 5 MB
- Admin role required

### Login
- Username (string, case-insensitive) must be provided
- Password is required in all cases

### User Creation

Name, Username, Password, Department, and Phone are all required
Role must be either Employee or Admin
Username must be unique (case-insensitive)
Username is stored in lowercase
Password is BCrypt hashed before storage
No self-registration — admin access required

---

## Cutoff Times
| Meal Type | Cutoff Time |
|-----------|-------------|
| Breakfast | 8:30 AM |
| Lunch | 10:30 AM |
| Evening Snacks | 3:00 PM |
| Dinner | 5:00 PM |

Cutoff enforcement uses the server machine's local system clock. Server timezone must be correctly set to IST (UTC+5:30) for accurate enforcement.

---

## Valid Field Values

### Canteen Locations
- Central Canteen
- Administrative Building
- Central Control Room
- Central Workshop

### Meal Types
- Breakfast
- Lunch
- Evening Snacks
- Dinner

### Booking For
- Self
- Guests

### Booking Status
- Confirmed
- Cancelled

### Menu Change Actions (Admin)
- Add
- Update
- Delete

---

## Seeded Dummy Data

### Employee Accounts
| EmployeeID | Name | Username | Role | Password |
|------------|------|----------|------|----------|
| 1 | Farhaz Rahman | farhaz.rahman | Admin | admin123 |
| 2 | Priya Sharma | priya.sharma | Employee | employee123 |
| 3 | Amit Das | amit.das | Employee | employee123 |

### Pricing Values
| MealType | BaseCost | PaneerSurcharge | NonVegSurcharge |
| Breakfast| 30 | 0 | 0 |
| Lunch | 45 | 25 | 25 |
| Evening Snacks | 30 | 0 | 0 |
| Dinner | 45 | 25 | 25 |

### Menu Items
28 items seeded across 7 days × 4 meal types, each with a `DisplayOrder` field preserving sequence within each meal slot. Items include realistic Indian canteen meals — Idli, Sambar, Poha, Rice, Dal, Roti, various curries, snacks, and desserts.

---

## Photo Upload System

Photos for Today's Special and Menu Items are uploaded directly from the admin's device (camera or gallery) rather than referenced by external URL.

**Upload flow:**
1. Admin selects or captures a photo in the app
2. App sends the image file to `POST /api/upload/photo`
3. Backend validates format and size, saves to `wwwroot/uploads/` with a GUID filename
4. Backend returns a relative path e.g. `/uploads/3f2a1b4c-xxxx.jpg`
5. App includes this path as `photoUrl` when creating or updating a Special or Menu Item
6. To display: prefix the stored path with backend base URL e.g. `http://[ip]:[port]/uploads/xxx.jpg`

Photos are currently stored on local disk. Migration to cloud storage (e.g. Azure Blob Storage) is planned for production without changing the API contract.

---

## ngrok Tunnel Setup (Cloud Testing)

ngrok is used to expose the locally running backend over HTTPS for remote frontend integration testing.

**Current session URL:**
```
https://provoke-factoid-crummiest.ngrok-free.dev
```

Note: This URL changes every time ngrok is restarted on the free plan.

**Required header for API calls through ngrok:**
```
ngrok-skip-browser-warning: true
```

**To start a new session:**
```
dotnet run --launch-profile http    (in backend terminal)
ngrok http [port]                   (in separate terminal)
```

Share the new Forwarding URL with the frontend team after each restart.

---

## Project File Structure

```
CanteenAPI/
├── Controllers/
    ├── AnnouncementsController.cs
│   ├── AuthController.cs
│   ├── BookingsController.cs
    ├── MealPricingController.cs
│   ├── MenuController.cs
│   ├── MenuAdminController.cs
    ├── NotificationsController.cs
│   ├── SpecialsController.cs
│   ├── UploadController.cs
    └── UsersController.cs
├── Data/
│   ├── AppDbContext.cs
│   └── DbSeeder.cs
├── DTOs/
    ├── AnnouncementDTOs.cs
│   ├── AuthDTOs.cs
│   ├── BookingDTOs.cs
    ├── MealPricingDTOs.cs
│   ├── MenuDTOs.cs
    ├── NotificationDTOs.cs
│   ├── SpecialDTOs.cs
    └── UserDTOs.cs
├── Migrations/
├── Models/
    ├── Announcement.cs
│   ├── Booking.cs
│   ├── Employee.cs
    ├── MealPricing.cs
│   ├── MenuItem.cs
    ├── Notification.cs
│   └── TodaysSpecial.cs
├── Properties/
│   └── launchSettings.json
├── wwwroot/
│   └── uploads/
├── appsettings.json
├── appsettings.Development.json
├── CanteenAPI.csproj
└── Program.cs
```

---

## Known Limitations and Pending Items

### Cosmetic (Non-blocking)
- `publishedBy` field in Today's Special response returns empty string — navigation property needs `.Include(s => s.CreatedBy)` in SpecialsController queries

### Pending — IT Integration
- Production SQL Server connection (server address and credentials pending from IT)
- Employee table integration — (use existing organization employee table directly); requires IT to share production table schema used by existing system
- Network firewall rules between backend server and production database server

### Pending — Future Enhancements
- Cloud storage for uploaded photos (Azure Blob Storage or equivalent)
- HTTPS/SSL certificate for production deployment
- Rate limiting and abuse protection
- Structured logging and monitoring
- Automated test suite (unit + integration tests for booking validation and cutoff logic)
- Push notifications for booking confirmation and cutoff reminders
- Special Meal HR approval workflow
- QR code based meal collection token
- Weekly meal subscription (recurring bookings)
- iOS frontend support
- Regional language support

---

## Local Development Setup (Quick Reference)

```
Prerequisites:
- .NET 8 SDK
- SQL Server Express 2022
- SSMS (optional but recommended)
- dotnet-ef CLI: dotnet tool install --global dotnet-ef --version 8.0.0

Clone and run:
git clone [repo-url]
cd CanteenAPI
dotnet restore
dotnet ef database update
dotnet run --launch-profile http

Swagger UI: http://localhost:[port]/swagger

For network access from phone (same WiFi):
- Find IPv4: ipconfig
- Allow port: netsh advfirewall firewall add rule name="CanteenAPI" dir=in action=allow protocol=TCP localport=[port]
- Frontend BASE_URL: http://[ipv4]:[port]/api

For remote access via ngrok:
ngrok http [port]
Frontend BASE_URL: https://[ngrok-url]/api
Add header: ngrok-skip-browser-warning: true
```

---

*Developed as part of PSU Internship Project, June 2026.*
*Backend: Farhaz Rahman*
*Frontend: Grihshant Manas Dutta*