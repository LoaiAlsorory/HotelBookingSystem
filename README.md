# Hotel Booking System (HBS)
## Enterprise Multi-Tier Hotel Reservation Platform

### Academic Project Information
- **Institution:** Al-Hikma University - Faculty of Engineering and Information Technology
- **Department:** Information Technology (IT)
- **Course:** Field Training Course (Comprehensive Enterprise SDLC Simulation)
- **Academic Year:** 2026 - 2027
- **Course Supervisor:** Prof. Ibrahim Ahmed Al-Baltah
- **Developer / Student:** Loai Alsorory
- **Target Framework:** .NET 10.0 (ASP.NET Core 10)

---

## 1. Executive Summary and Architecture Overview

The Hotel Booking System is an enterprise-grade, multi-tier web application engineered to simulate full-scale hospitality management operations. Designed following the principles of **Clean Architecture** and the **Repository Pattern**, the solution enforces strict separation of concerns, decoupling business logic from infrastructure implementations and user interfaces.

`	ext
HotelBookingSystem/
├── src/
│   ├── HotelBookingSystem.Domain/          # Enterprise Entities, Value Objects, and Domain Contracts
│   ├── HotelBookingSystem.Application/     # Application Services, DTOs, Repository Interfaces, Validation
│   ├── HotelBookingSystem.Infrastructure/  # EF Core DbContext, Repository Implementations, Migrations
│   ├── HotelBookingSystem.API/             # RESTful Web API Controllers, JWT Auth, Swagger Documentation
│   └── HotelBookingSystem.Web/             # ASP.NET Core MVC Portal, Razor Views, Bootstrap 5 UI
└── HotelBookingSystem.sln
`

### Dependency Flow
`	ext
[ HotelBookingSystem.Web ]      [ HotelBookingSystem.API ]
           │                                 │
           └──────────────┬──────────────────┘
                          ▼
             [ HotelBookingSystem.Application ]
                          │
                          ▼
             [ HotelBookingSystem.Domain ]
                          ▲
                          │
           [ HotelBookingSystem.Infrastructure ]
`

---

## 2. Compliance with Course Guide Requirements

| # | Course Requirement | Technical Implementation Details | Verification Reference |
|---|--------------------|----------------------------------|------------------------|
| 1 | At least three connected tables | 4 fully relational entities: Users, Hotels, Rooms, and Bookings with strict foreign key constraints and cascade rules. | Domain/Entities/*.cs |
| 2 | At least one table has images | Both Hotels and Rooms contain image URL attributes with dedicated visual cards in MVC and API payloads. | Hotel.cs, Room.cs |
| 3 | Main Dashboard | Interactive management dashboard providing real-time aggregation metrics (total hotels, rooms, active bookings, revenue, recent transactions). | Web/Views/Home/Index.cshtml |
| 4 | Entity Views (Index, Details, Create, Edit, Delete) + Input Validation | Full CRUD views implemented for all 4 entities with server-side DataAnnotations, client-side unobtrusive validation, and business rule enforcement. | Web/Views/{Entity}/*.cshtml |
| 5 | Main Web Page for Displaying Information | Front-facing hotel directory featuring multi-attribute search, category filtering, capacity indicators, and room booking shortcuts. | Web/Views/Home/Index.cshtml |
| 6.a | Bootstrap | Bootstrap 5.3 (RTL compiled) integrated across all views for structured responsiveness. | Web/Views/Shared/_Layout.cshtml |
| 6.b | Bootstrap Icons | Vector iconography applied consistently across navigation menus, status tags, and action buttons. | ootstrap-icons.min.css |
| 6.c | Custom Typography | Google Fonts Cairo loaded and globally configured as the standard typography engine for enhanced Arabic readability. | _Layout.cshtml, site.css |
| 6.d | CSS Variables | Centralized design tokens defined in :root (--hbs-primary, --hbs-gold, --hbs-sidebar-bg, --hbs-card-radius). | wwwroot/css/site.css |
| 6.e | Bootstrap Modal | Reusable asynchronous modal components implemented for sensitive operations including record deletions. | _DeleteModal.cshtml |
| 6.f | DataTable.js | Client-side pagination, instant multi-column search, and sorting enabled on all administrative data grids. | wwwroot/js/site.js |
| 7 | GitHub Branches and Merging | Feature-based branch workflow (eature/w7-mvc-web, eature/w7-auth-login, eature/luxury-redesign) merged into main. | Git commit and merge history |

---

## 3. Database Schema and Relational Model

The relational persistence tier is powered by Microsoft SQL Server via Entity Framework Core:

`	ext
+----------------+          +----------------+
|     Users      |          |     Hotels     |
+----------------+          +----------------+
| Id (PK)        |          | Id (PK)        |
| FullName       |          | Name           |
| Email (Unique) |          | City           |
| PasswordHash   |          | Address        |
| Role           |          | Description    |
| CreatedAt      |          | ImageUrl       |
+-------+--------+          | Rating         |
        | 1                 +-------+--------+
        |                           | 1
        | N                         | N
+-------+--------+          +-------+--------+
|    Bookings    |    N     |     Rooms      |
+----------------+----------+----------------+
| Id (PK)        |        1 | Id (PK)        |
| UserId (FK)    |<---------| HotelId (FK)   |
| RoomId (FK)    |          | RoomNumber     |
| CheckInDate    |          | Type           |
| CheckOutDate   |          | PricePerNight  |
| TotalPrice     |          | Capacity       |
| Status         |          | IsAvailable    |
| CreatedAt      |          | ImageUrl       |
+----------------+          +----------------+
`

---

## 4. RESTful Web API Specifications (W5 & W6)

The HotelBookingSystem.API module exposes secured HTTP endpoints conforming to REST conventions, documented through Swagger/OpenAPI specifications.

### Core Endpoints

| Resource | HTTP Method | Route | Description | Auth Required |
|----------|-------------|-------|-------------|---------------|
| Auth | POST | /api/auth/register | Register new user account | No |
| Auth | POST | /api/auth/login | Authenticate credentials and return JWT bearer token | No |
| Hotels | GET | /api/hotels | Retrieve all registered hotels | No |
| Hotels | GET | /api/hotels/{id} | Retrieve specific hotel by primary key | No |
| Hotels | POST | /api/hotels | Create new hotel record | Yes (Admin) |
| Hotels | PUT | /api/hotels/{id} | Update existing hotel details | Yes (Admin) |
| Hotels | DELETE | /api/hotels/{id} | Remove hotel record | Yes (Admin) |
| Rooms | GET | /api/rooms | Retrieve rooms (supports optional hotelId filtering) | No |
| Rooms | GET | /api/rooms/{id} | Retrieve room details | No |
| Rooms | POST | /api/rooms | Add room to designated hotel | Yes (Admin) |
| Rooms | PUT | /api/rooms/{id} | Update room specifications | Yes (Admin) |
| Rooms | DELETE | /api/rooms/{id} | Delete room record | Yes (Admin) |
| Bookings | GET | /api/bookings | List bookings (supports userId parameter) | Yes |
| Bookings | GET | /api/bookings/{id} | Get booking details | Yes |
| Bookings | POST | /api/bookings | Create booking with concurrency/conflict validation | Yes |
| Bookings | PUT | /api/bookings/{id}/status | Update booking lifecycle status | Yes |
| Bookings | DELETE | /api/bookings/{id} | Cancel/delete booking record | Yes |
| Users | GET | /api/users | List system users | Yes (Admin) |
| Users | GET | /api/users/{id} | Retrieve user profile by identifier | Yes |

### Validation and Integrity Rules
- Temporal bounds enforcement: CheckOutDate > CheckInDate and dates cannot be historical.
- Anti-collision logic: Overlapping reservation intervals on identical room assets are rejected at the service layer (DoubleBookingException).
- Cryptographic security: User passwords hashed via PBKDF2 with unique cryptographic salt.
- Stateless authentication: Protected endpoints enforce RFC 7519 JSON Web Token verification.

---

## 5. Web MVC Interface (W7)

The HotelBookingSystem.Web assembly provides a responsive enterprise portal:
- **Interactive Metric Panels:** Real-time financial and occupancy key performance indicators (KPIs).
- **RTL-Native Design System:** Tailored for Arabic typography with logical property alignment.
- **Client and Server Data Validation:** Model state validation combined with immediate visual feedback.
- **Safe State Transitions:** CSRF anti-forgery token verification implemented on all state-mutating requests.

---

## 6. Decoupled Architecture Notice (Flutter Mobile App)

In accordance with official directives issued by the course supervisor, **Prof. Ibrahim Ahmed Al-Baltah**, the mobile client tier has been extracted into a completely independent project and dedicated GitHub repository:

- **Mobile Client Repository:** [https://github.com/LoaiAlsorory/HotelBookingSystem-Flutter](https://github.com/LoaiAlsorory/HotelBookingSystem-Flutter)
- The mobile client operates as a decoupled consumer communicating exclusively through the RESTful API endpoints exposed by this project.

---

## 7. Installation and Execution Guide

### Prerequisites
- .NET 10.0 SDK
- Microsoft SQL Server or SQL Server LocalDB
- Visual Studio 2022 / VS Code / JetBrains Rider

### Setup Steps
1. Clone the repository:
   `ash
   git clone https://github.com/LoaiAlsorory/HotelBookingSystem.git
   cd HotelBookingSystem
   `
2. Restore package dependencies:
   `ash
   dotnet restore
   `
3. Configure the database connection string in src/HotelBookingSystem.Web/appsettings.json and src/HotelBookingSystem.API/appsettings.json (defaults to SQL Server LocalDB).
4. Run the Web API backend:
   `ash
   dotnet run --project src/HotelBookingSystem.API
   `
5. In a separate terminal, launch the ASP.NET Core MVC web portal:
   `ash
   dotnet run --project src/HotelBookingSystem.Web
   `
6. Access points:
   - Web Portal: http://localhost:5200 (or designated launch URL)
   - Swagger Documentation: http://localhost:5242/swagger