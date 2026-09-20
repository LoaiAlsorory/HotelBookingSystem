import os

content = """# REPUBLIC OF YEMEN
## Ministry of Education and Scientific Research
### AL-HIKMA UNIVERSITY - Faculty of Engineering and Information Technology
#### Department of Information Technology

---

# Comprehensive Final Software Project Documentation Report
## Full-Stack Enterprise Hotel Booking & Management System
### Field Training Course Guide (Academic Year 2026 - 2027)

| Attribute | Details |
|---|---|
| **Course Name** | Field Training Course (Individual Enterprise SDLC Simulation) |
| **Course Code** | IT-492 |
| **Course Supervisor** | Prof. Ibrahim Ahmed Al-Baltah |
| **Student / Software Engineer** | Loai Alsorory |
| **System Title** | Enterprise Multi-Tier Hotel Booking System |
| **Core Frameworks** | ASP.NET Core 10.0 / Entity Framework Core 10 / Flutter 3.x |
| **Backend Repository** | https://github.com/LoaiAlsorory/HotelBookingSystem |
| **Mobile Client Repository** | https://github.com/LoaiAlsorory/HotelBookingSystem-Flutter |
| **Document Version** | Version 1.0.0 (Final Comprehensive Release) |
| **Date of Submission** | September 2026 |

---

## Table of Contents
1. Executive Summary and Architecture Overview
2. Part 1: Environmental Deployment and Installation Guide (W10.1)
3. Part 2: Comprehensive User Manual and Screen Guide (W10.2)
4. Part 3: Testing and Quality Assurance Verification (W10.3)
5. Part 4: Cumulative Project Deliverables Synthesis (W2 - W9)
6. Part 5: Conclusion, Engineering Challenges, and Future Enhancements (W10.4)
7. Part 6: Academic and Technical References (W10.5)

---

## 1. Executive Summary and Architecture Overview

This comprehensive documentation report serves as the final technical submission for the Field Training Course at Al-Hikma University, conducted under the supervision of Prof. Ibrahim Ahmed Al-Baltah. Rather than a theoretical exercise, the project was executed under a strict, individual Full-Stack Software Engineering simulation simulating enterprise-grade production requirements across the entire Software Development Life Cycle (SDLC).

The developed solution is a distributed, multi-tier enterprise hotel management and reservation platform comprising two completely decoupled components:
1. **Core Backend & Administrative Web Portal (`HotelBookingSystem`):** Built using **ASP.NET Core 10** following **Clean Architecture** and the **Repository Pattern**. It hosts the relational data layer, secure RESTful API endpoints, and a server-rendered administrative MVC web portal.
2. **Autonomous Mobile Client (`HotelBookingSystem-Flutter`):** Built using Google's **Flutter** framework following the **Provider Pattern**. It serves as an independent mobile client communicating exclusively via JSON-over-HTTP with the backend API.

```text
[ Administrative MVC Web Portal ]       [ Independent Flutter Mobile Client ]
(Bootstrap 5 RTL + DataTables.js)       (Provider Pattern + Cairo Typography)
               │                                          │
               │ Direct Application Context               │ RESTful HTTP/JSON
               ▼                                          ▼
     [ HotelBookingSystem.Application ] ◄─── [ HotelBookingSystem.API ]
               │
               ▼
     [ HotelBookingSystem.Domain ]
               ▲
               │ Entity Framework Core 10
     [ HotelBookingSystem.Infrastructure ]
               │
               ▼
     [ Microsoft SQL Server Database ]
```

---

## 2. Part 1: Environmental Deployment and Installation Guide (W10.1)

This section provides definitive, step-by-step instructions for engineers and academic evaluators to configure, compile, and execute the complete enterprise system from source code.

### 2.1 Hardware and Software Prerequisites

| Component | Minimum Specification | Recommended Specification | Notes |
|---|---|---|---|
| Operating System | Windows 10 / 11 (64-bit) | Windows 11 Pro (64-bit) | Linux / macOS supported via .NET Core runtime |
| Processor | Dual-Core 2.0 GHz | Quad-Core Intel Core i5/i7 or AMD Ryzen | Required for virtualization/emulation |
| Memory (RAM) | 8.00 GB | 16.00 GB | Allows simultaneous execution of API, Web, and Flutter |
| Disk Storage | 5.00 GB free space | 15.00 GB SSD storage | Required for SDKs, DB files, and dependencies |
| .NET Runtime | .NET 10.0 SDK | .NET 10.0 SDK (x64) | Core framework for API and MVC projects |
| Database Server | SQL Server LocalDB | Microsoft SQL Server 2022 Express | LocalDB is pre-configured by default |
| Mobile Framework | Flutter SDK 3.19.x | Flutter SDK 3.22.x or later | Configured with Dart 3.x |
| Client Runtimes | Google Chrome (Web) | Google Chrome + Android SDK (API 34) | For testing mobile and web targets |
| Version Control | Git CLI 2.40+ | Git CLI 2.45+ | For repository operations and branch merging |

---

### 2.2 Database Configuration and Seeding Strategy

The project utilizes Entity Framework Core 10 code-first modeling. The persistence connection string is located in both `appsettings.json` files:
- `src/HotelBookingSystem.API/appsettings.json`
- `src/HotelBookingSystem.Web/appsettings.json`

Default Connection String:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HotelBookingDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

#### Automated Initialization Engine
Manual SQL migration script execution is optional. The application implements an automated startup initialization sequence in `Program.cs`:
1. `context.Database.EnsureCreated()` verifies the physical database existence and compiles the 4 relational schemas.
2. `DbSeeder.SeedAsync(context)` checks if the database is empty. If unseeded, it populates:
   - Initial administrative and customer credentials with salted PBKDF2 hashes.
   - 12 unique luxury hotel records across major Arab capitals, each mapped to a distinct high-resolution image URL.
   - 48 distinct hotel room records with realistic pricing, capacities, and dedicated room photography.
   - Initial sample reservations verifying status transitions.

---

### 2.3 Backend API and Web Portal Deployment Steps

#### Step 1: Clone the Backend Repository
```bash
git clone https://github.com/LoaiAlsorory/HotelBookingSystem.git
cd HotelBookingSystem
```

#### Step 2: Restore Dependencies and Build Solution
```bash
dotnet restore HotelBookingSystem.sln
dotnet build HotelBookingSystem.sln --configuration Release --nologo
```

#### Step 3: Launch the RESTful Web API Service
Open a dedicated terminal window:
```bash
cd src/HotelBookingSystem.API
dotnet run
```
- API Base Address: `http://localhost:5242`
- Interactive OpenAPI / Swagger UI: `http://localhost:5242/swagger`

#### Step 4: Launch the Administrative Web Portal (MVC)
Open a separate terminal window:
```bash
cd src/HotelBookingSystem.Web
dotnet run
```
- Web Portal Address: `http://localhost:5200` (or displayed launch port)
- Default Administrative Credentials:
  - Email: `admin@hotel.com`
  - Password: `Password123!`

---

### 2.4 Standalone Flutter Mobile Client Deployment Steps

In compliance with supervisor directives, the mobile client is deployed from its isolated repository.

#### Step 1: Clone the Mobile Repository
```bash
git clone https://github.com/LoaiAlsorory/HotelBookingSystem-Flutter.git
cd HotelBookingSystem-Flutter
```

#### Step 2: Fetch Flutter Packages
```bash
flutter pub get
```

#### Step 3: Verify API Endpoint Target
Inspect `lib/core/constants/api_constants.dart`. The default configuration targets `http://localhost:5242/api` for Chrome/Web execution or local desktop runs. For physical Android devices connected via USB, replace `localhost` with your workstation's LAN IP address (e.g., `http://192.168.1.100:5242/api`).

#### Step 4: Execute Application
```bash
# Launch on Web browser (Instant verification)
flutter run -d chrome

# Launch on connected Android physical device / emulator
flutter run
```

---

## 3. Part 2: Comprehensive User Manual and Screen Guide (W10.2)

The system provides dual user interfaces: a desktop-optimized administrative web portal and a mobile application for guests and on-the-go managers.

### 3.1 User Access Roles and Permissions

| Role | Target Interface | Capabilities |
|---|---|---|
| **System Administrator** | Web Portal & Mobile App | Full CRUD on Hotels, Rooms, Users, and Bookings. Global metrics access, status overrides, and system configuration. |
| **Registered Customer** | Mobile App & Web Front | Profile management, browsing hotels, filtering room availability, creating reservations, and viewing personal booking history. |
| **Guest / Anonymous** | Web Front & Mobile App | Viewing public hotel catalogs, inspecting room specifications, and registering new accounts. |

---

### 3.2 Administrative Web Portal Walkthrough

#### 1. Security Authentication & Split-Screen Login (`/Account/Login`)
The authentication interface features a luxury split-screen design. The left panel showcases high-end hospitality imagery and brand messaging, while the right panel provides input validation for email and password. Upon successful verification, an encrypted authentication cookie is issued.

#### 2. Executive Management Dashboard (`/Home/Index`)
The dashboard aggregates operational data through 5 primary visual sections:
- **KPI Analytic Counters:** Total Registered Hotels, Total Available Rooms, Total Active Reservations, and Gross Financial Revenue.
- **Quick-Access Action Grid:** Instant shortcuts to register new hotels, add rooms, or view pending bookings.
- **Recent Bookings Table:** Real-time log of the latest reservations with status badges (Confirmed, Pending, Cancelled).
- **Public Hotel Showcase:** Visual grid displaying featured properties with dynamic pricing indicators.

#### 3. Hotel Management Module (`/Hotels`)
- **Index View:** Comprehensive data grid enhanced with DataTables.js, supporting instantaneous search across names, cities, and addresses.
- **Create View:** Form with validation enforcing non-empty names, allowable rating ranges (1.0 to 5.0), and valid image URLs.
- **Edit View:** In-place modification of descriptions, locations, and media assets.
- **Details View:** Deep-dive card view featuring property overview, rating stars, and associated room inventories.
- **Delete Modal:** Secure deletion confirmation triggered via an asynchronous Bootstrap Modal, preventing accidental record loss.

#### 4. Room Inventory Module (`/Rooms`)
- **Capacity & Type Indicators:** Visual distinction between Single, Double, Suite, and Deluxe options.
- **Availability State Control:** Toggle switches to immediately mark rooms as Available or Occupied.
- **Pricing Configuration:** Decimal validation ensuring positive nightly rates.

#### 5. Booking Lifecycle Module (`/Bookings`)
- **Date Boundary Picker:** Date inputs enforcing future arrival dates and checkout dates strictly greater than checkin.
- **Automatic Financial Calculation:** Calculation engine computing `NightCount * PricePerNight = TotalPrice`.
- **Status Workflow Transition:** Ability to update status between Confirmed, Pending, and Cancelled.

---

### 3.3 Flutter Mobile Application Walkthrough

The mobile app delivers a fluid, responsive client adhering to Material 3 design and right-to-left (RTL) ergonomics:

#### 1. Main Dashboard Screen (`HomeScreen`)
- Top hero banner welcoming the authenticated user with current date and time.
- Animated stat cards displaying live counts for Hotels, Rooms, and Bookings.
- Horizontal carousel displaying top-rated hotel recommendations with cached network images.
- Quick navigation shortcuts allowing single-tap transitions to all sub-modules.

#### 2. Navigation Architecture (`MainScreen`)
- **Right-to-Left Drawer (القائمة الجانبية):** Smooth slide-out menu containing user account details, role badge, navigation links, and session logout.
- **Persistent Bottom Navigation Bar:** 5 dedicated destinations (Dashboard, Users, Hotels, Rooms, Bookings) styled with Cairo font and active color highlights.
- **Adaptive Responsive Frame:** An internal geometry adapter ensuring consistent mobile phone proportions without viewport clipping on large screens.

#### 3. Hotel Catalog & Property Inspection (`HotelsScreen` & `HotelDetailsScreen`)
- Search bar with real-time text query filtering across hotel names and cities.
- Interactive cards with high-definition imagery, location badges, and price starting points.
- Detailed view presenting full property description, amenities list, and direct room booking integration.

#### 4. Room Booking Bottom Sheet (`SheetModal`)
- Triggered by clicking **Book Now** on any room card.
- Opens an interactive modal sheet (`showModalBottomSheet`) displaying room specifications, calendar date selectors, guest count counter, total price summary, and a confirmation button.
- Submits the payload asynchronously to the Web API with immediate feedback via floating SnackBar notifications.

---

## 4. Part 3: Testing and Quality Assurance Verification (W10.3)

Comprehensive testing was conducted across all tiers to validate architectural integrity, data consistency, and functional correctness.

### 4.1 RESTful Web API Test Results (Swagger & Postman)

A dedicated test suite containing 28 test cases was executed against the API using Postman and Swagger UI.

| Endpoint | Method | Test Objective | Request Payload | Expected Status | Result |
|---|---|---|---|---|---|
| `/api/auth/login` | POST | Authenticate Admin | Valid JSON credentials | `200 OK` (with JWT token) | Passed |
| `/api/auth/login` | POST | Reject Bad Password | Invalid password string | `401 Unauthorized` | Passed |
| `/api/hotels` | GET | List all hotels | Empty | `200 OK` (Array of 12 hotels) | Passed |
| `/api/hotels/{id}` | GET | Retrieve single hotel | ID: 1 | `200 OK` (Hotel DTO) | Passed |
| `/api/hotels/99999` | GET | Non-existent entity | ID: 99999 | `404 Not Found` | Passed |
| `/api/hotels` | POST | Create hotel (Valid) | Complete Hotel DTO | `201 Created` | Passed |
| `/api/hotels` | POST | Reject Empty Name | Name: `""` | `400 Bad Request` | Passed |
| `/api/rooms` | GET | Filter rooms by Hotel | Query: `?hotelId=1` | `200 OK` (4 rooms) | Passed |
| `/api/bookings` | POST | Create valid booking | Valid RoomId, dates | `201 Created` | Passed |
| `/api/bookings` | POST | Reject Past Dates | CheckIn: `2020-01-01` | `400 Bad Request` | Passed |
| `/api/bookings` | POST | Reject Double Booking | Overlapping dates on booked room | `409 Conflict` | Passed |

---

### 4.2 HTTP Status Code Distribution Matrix

The Web API strictly enforces standard RFC HTTP status codes:
- **`200 OK`:** Successful retrieval or modification of existing resources.
- **`201 Created`:** Successful resource generation, returning the created entity and `Location` header.
- **`400 Bad Request`:** Payload validation failure, malformed JSON, or invalid date chronology.
- **`401 Unauthorized`:** Missing or expired JSON Web Token on protected endpoints.
- **`404 Not Found`:** Resource identifier does not match any record in the database.
- **`409 Conflict`:** Business rule collision, specifically triggered on concurrent double-booking attempts.

---

### 4.3 Input Validation Verification Matrix

Input integrity is enforced at both client and server boundaries:

| Entity | Attribute | Applied Validation Rule | Failure Test Case | System Response |
|---|---|---|---|---|
| `User` | `Email` | `[Required]`, `[EmailAddress]` | `"invalid-email-format"` | "يرجى إدخال بريد إلكتروني صحيح" |
| `User` | `FullName` | `[Required]`, `[StringLength(100, Min=3)]` | `"AB"` | "يجب أن يحتوي الاسم على 3 أحرف على الأقل" |
| `Hotel` | `Rating` | `[Range(1.0, 5.0)]` | `6.5` | "يجب أن يكون التقييم بين 1 و 5" |
| `Room` | `PricePerNight` | `[Range(1, 1000000)]` | `-50.00` | "يجب أن يكون السعر رقماً موجباً أكبر من الصفر" |
| `Room` | `Capacity` | `[Range(1, 20)]` | `0` | "سعة الغرفة يجب أن تتسع لشخص واحد على الأقل" |
| `Booking`| `CheckOutDate` | `IValidatableObject` rule | CheckOut <= CheckIn | "تاريخ المغادرة يجب أن يكون بعد تاريخ الوصول" |

---

### 4.4 Business Logic & Concurrency Testing (Anti-Collision Engine)

The core business invariant is the prevention of double bookings:
```csharp
// Business Logic Verification in BookingRepository / BookingService:
bool isDoubleBooked = await _context.Bookings.AnyAsync(b =>
    b.RoomId == request.RoomId &&
    b.Status != BookingStatus.Cancelled &&
    ((request.CheckInDate >= b.CheckInDate && request.CheckInDate < b.CheckOutDate) ||
     (request.CheckOutDate > b.CheckInDate && request.CheckOutDate <= b.CheckOutDate) ||
     (request.CheckInDate <= b.CheckInDate && request.CheckOutDate >= b.CheckOutDate)));
```
- **Test Case Execution:** Room 101 was reserved from Oct 10 to Oct 15. A concurrent reservation request was submitted for Room 101 from Oct 12 to Oct 18.
- **Verification Result:** The second request was blocked with HTTP 409 Conflict and message: *"عذراً، هذه الغرفة محجوزة بالفعل في الفترة المحددة"*. The database state remained consistent with zero conflicting overlapping records.

---

## 5. Part 4: Cumulative Project Deliverables Synthesis (W2 - W9)

This section correlates each semester week milestone with its concrete implementation:

### Week 2: Project Proposal and Requirements Engineering
- Problem Statement: Traditional manual reservation mechanisms suffer from data fragmentation and concurrency conflicts.
- Scope: Enterprise hotel booking system with multi-property cataloging and automated reservation workflows.
- Requirements: 12 Functional Requirements (FR1 to FR12) and 6 Non-Functional Requirements (NFR1 to NFR6) covering security, availability, and response time.

### Week 3: Software Design & Clean Architecture
- Implemented **Clean Architecture** with strict dependency inversion.
- Domain models created with zero external framework dependencies.
- Diagrams generated: Architecture Layer Diagram, Use Case Model, Sequence Diagram for Booking Workflow, and Entity Relational Class Diagram.

### Week 4: UI/UX Prototyping
- Developed high-fidelity interactive prototypes in Adobe XD (`XD.xd`).
- Curated an enterprise brand identity: Indigo (`#4F46E5`), Deep Violet (`#7C3AED`), and Luxury Gold (`#C9971F`) accents.
- Responsive layout strategies tailored for Arabic Right-to-Left (RTL) reading patterns.

### Week 5: Web API Development (ASP.NET Core 10)
- Engineered 18 secure endpoints across Auth, Hotels, Rooms, Bookings, and Users.
- Configured JSON serialization policies, global exception middleware, and CORS policies.

### Week 6: Automated API Testing & Documentation
- Comprehensive Postman Collection created with dynamic environment variables (`{{baseUrl}}`, `{{token}}`, `{{hotelId}}`).
- Full Swagger/OpenAPI interactive interface enabled for zero-code manual testing.

### Week 7: ASP.NET Core MVC Application
- Server-rendered Razor Views with Bootstrap 5 RTL, Bootstrap Icons, and Cairo typography.
- Integrated DataTables.js for client-side table indexing and search.
- Reusable Bootstrap Modal dialogs for operational confirmations.

### Week 8: Flutter Mobile Application
- Cross-platform client with Provider state management.
- Implemented Material 3 Drawer, BottomNavigationBar, AppBar, and SheetModal components.
- Responsive phone frame engine resolving multi-screen ratio discrepancies.

### Week 9: GitHub Source Control & Branching Strategy
- Git branching model: `main`, feature branches (`feature/w7-mvc-web`, `feature/w8-mobile-ui-and-api`), and non-fast-forward merge commits (`--no-ff`).
- Decoupled into two independent GitHub repositories per supervisor requirement.

---

## 6. Part 5: Conclusion, Engineering Challenges, and Future Enhancements (W10.4)

### 6.1 Project Conclusion
The Field Training Course simulation successfully mirrored real-world software enterprise development. The candidate single-handedly executed every lifecycle phase: requirements gathering, architectural modeling, database persistence, secure backend engineering, web portal design, mobile client construction, quality assurance, and formal documentation. The resultant platform demonstrates production-ready code quality, strict separation of concerns, and robust error resilience.

### 6.2 Key Engineering Challenges and Resolutions

1. **Micro-Client Decoupling (Supervisor Directive):**
   - *Challenge:* The mobile app was initially structured within the solution folder. The supervisor instructed that Flutter must reside in an independent project and repository.
   - *Resolution:* The Flutter client was isolated into a dedicated project directory, git version control was initialized with distinct branches and merge history, build caches were pruned, and an independent repository was published to GitHub.
2. **Temporal Synchronization and UTC Standardization:**
   - *Challenge:* Booking dates sent from different client timezones resulted in intermittent 1-day discrepancies in SQL Server date comparisons.
   - *Resolution:* All date inputs are normalized to UTC `DateTime.SpecifyKind(date, DateTimeKind.Utc)` at the application layer boundary prior to persistence.
3. **Responsive Mobile Viewport Geometry:**
   - *Challenge:* Viewing mobile interfaces across variable aspect ratios caused yellow/black striping (overflow errors).
   - *Resolution:* Designed a custom `ResponsivePhoneFrame` widget using `LayoutBuilder` and `SingleChildScrollView` to provide fluid adaptability.

### 6.3 Future Engineering Roadmap
1. **Financial Payment Gateway Integration:** Connecting Stripe, PayPal, and local bank payment APIs for direct transaction clearing.
2. **Push Notification Service:** Integrating Firebase Cloud Messaging (FCM) to deliver instant reservation confirmation and check-in reminders.
3. **Multi-Language Localization (i18n):** Adding dynamic toggle between Arabic and English across both web and mobile clients.
4. **AI-Driven Recommendation Engine:** Implementing collaborative filtering to suggest personalized hotel selections based on past user preferences.

---

## 7. Part 6: Academic and Technical References (W10.5)

1. **Martin, Robert C. (Uncle Bob)** (2017). *Clean Architecture: A Craftsman's Guide to Software Structure and Design*. Prentice Hall.
2. **Microsoft Corporation** (2025/2026). *ASP.NET Core 10.0 Documentation and Architectural Guidance*. Microsoft Learn.
3. **Entity Framework Core Team** (2025). *Entity Framework Core: Code-First Modeling and Concurrency Control*. Microsoft Docs.
4. **Google LLC** (2024/2026). *Flutter Framework Documentation: State Management via Provider Pattern*. Flutter.dev.
5. **Fielding, Roy Thomas** (2000). *Architectural Styles and the Design of Network-based Software Architectures* (REST Architectural Pattern). University of California, Irvine.
6. **Internet Engineering Task Force (IETF)** (2015). *RFC 7519: JSON Web Token (JWT)*. IETF Standards.
7. **Al-Hikma University** (2026). *Field Training Course Guide: Individual Full-Stack Software Development Project*. Department of Information Technology, Sana'a, Yemen.
"""

output_path = os.path.join(os.path.dirname(__file__), "W10_Final_Project_Documentation.md")
with open(output_path, "w", encoding="utf-8") as f:
    f.write(content.strip())

print("Saved successfully to:", output_path)
