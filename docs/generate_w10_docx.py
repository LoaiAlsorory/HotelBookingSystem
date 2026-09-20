import docx
from docx.shared import Inches, Pt, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.enum.table import WD_TABLE_ALIGNMENT, WD_ALIGN_VERTICAL
from docx.oxml import OxmlElement, parse_xml
from docx.oxml.ns import nsdecls, qn
import os

def set_cell_background(cell, fill_hex):
    tcPr = cell._tc.get_or_add_tcPr()
    shd = parse_xml(f'<w:shd {nsdecls("w")} w:fill="{fill_hex}"/>')
    tcPr.append(shd)

def set_cell_margins(cell, top=100, bottom=100, left=150, right=150):
    tcPr = cell._tc.get_or_add_tcPr()
    tcMar = parse_xml(f'<w:tcMar {nsdecls("w")}><w:top w:w="{top}" w:type="dxa"/><w:bottom w:w="{bottom}" w:type="dxa"/><w:left w:w="{left}" w:type="dxa"/><w:right w:w="{right}" w:type="dxa"/></w:tcMar>')
    tcPr.append(tcMar)

def set_table_borders(table, color="D1D5DB", sz="4"):
    tblPr = table._tbl.tblPr
    borders = parse_xml(
        f'<w:tblBorders {nsdecls("w")}>'
        f'<w:top w:val="single" w:sz="{sz}" w:space="0" w:color="{color}"/>'
        f'<w:bottom w:val="single" w:sz="{sz}" w:space="0" w:color="{color}"/>'
        f'<w:insideH w:val="single" w:sz="{sz}" w:space="0" w:color="{color}"/>'
        f'<w:insideV w:val="none"/>'
        f'<w:left w:val="none"/>'
        f'<w:right w:val="none"/>'
        f'</w:tblBorders>'
    )
    tblPr.append(borders)

def create_document():
    doc = docx.Document()
    
    # Page setup
    for section in doc.sections:
        section.top_margin = Inches(1.0)
        section.bottom_margin = Inches(1.0)
        section.left_margin = Inches(1.0)
        section.right_margin = Inches(1.0)
        section.page_width = Inches(8.5)
        section.page_height = Inches(11.0)
        
    # Styles
    styles = doc.styles
    normal_style = styles['Normal']
    normal_style.font.name = 'Segoe UI'
    normal_style.font.size = Pt(10.5)
    normal_style.font.color.rgb = RGBColor(0x1F, 0x29, 0x37) # Dark slate
    normal_style.paragraph_format.line_spacing = 1.15
    normal_style.paragraph_format.space_after = Pt(4)

    # -------------------------------------------------------------
    # COVER PAGE
    # -------------------------------------------------------------
    p_header = doc.add_paragraph()
    p_header.alignment = WD_ALIGN_PARAGRAPH.CENTER
    run_inst = p_header.add_run("REPUBLIC OF YEMEN\nMinistry of Education and Scientific Research\nAL-HIKMA UNIVERSITY\nFaculty of Engineering and Information Technology\nDepartment of Information Technology\n")
    run_inst.font.size = Pt(11)
    run_inst.font.bold = True
    run_inst.font.color.rgb = RGBColor(0x37, 0x41, 0x51)
    
    doc.add_paragraph().paragraph_format.space_after = Pt(24)
    
    p_title = doc.add_paragraph()
    p_title.alignment = WD_ALIGN_PARAGRAPH.CENTER
    run_title = p_title.add_run("Comprehensive Software Project Documentation Report\n")
    run_title.font.size = Pt(22)
    run_title.font.bold = True
    run_title.font.color.rgb = RGBColor(0x1E, 0x1B, 0x4B) # Deep indigo
    
    run_subtitle = p_title.add_run("Enterprise Multi-Tier Hotel Booking System\n")
    run_subtitle.font.size = Pt(15)
    run_subtitle.font.color.rgb = RGBColor(0x4F, 0x46, 0xE5) # Brand indigo
    run_subtitle.font.bold = True
    
    run_guide = p_title.add_run("Field Training Course (SDLC Enterprise Simulation 2026 - 2027)")
    run_guide.font.size = Pt(11)
    run_guide.font.italic = True
    run_guide.font.color.rgb = RGBColor(0x6B, 0x72, 0x80)

    doc.add_paragraph().paragraph_format.space_after = Pt(36)

    # Metadata Table
    meta_table = doc.add_table(rows=8, cols=2)
    meta_table.alignment = WD_TABLE_ALIGNMENT.CENTER
    set_table_borders(meta_table, "E5E7EB", "6")
    
    meta_data = [
        ("Course Name", "Field Training Course (Individual Full-Stack SDLC Simulation)"),
        ("Course Code", "IT-492"),
        ("Course Supervisor", "Prof. Ibrahim Ahmed Al-Baltah"),
        ("Student / Developer", "Loai Alsorory"),
        ("System Architecture", "Clean Architecture + Repository Pattern + Provider (Flutter)"),
        ("Backend Repository", "https://github.com/LoaiAlsorory/HotelBookingSystem"),
        ("Mobile Client Repository", "https://github.com/LoaiAlsorory/HotelBookingSystem-Flutter"),
        ("Submission Date", "September 2026 | Version 1.0.0 (Final Release)")
    ]
    
    for idx, (label, val) in enumerate(meta_data):
        row = meta_table.rows[idx]
        cell_lbl, cell_val = row.cells[0], row.cells[1]
        
        cell_lbl.width = Inches(2.3)
        cell_val.width = Inches(4.2)
        
        set_cell_background(cell_lbl, "F3F4F6")
        set_cell_margins(cell_lbl, 80, 80, 120, 120)
        set_cell_margins(cell_val, 80, 80, 120, 120)
        
        p0 = cell_lbl.paragraphs[0]
        r0 = p0.add_run(label)
        r0.font.bold = True
        r0.font.size = Pt(10)
        r0.font.color.rgb = RGBColor(0x1F, 0x29, 0x37)
        
        p1 = cell_val.paragraphs[0]
        r1 = p1.add_run(val)
        r1.font.size = Pt(10)
        r1.font.color.rgb = RGBColor(0x37, 0x41, 0x51)

    doc.add_page_break()

    # -------------------------------------------------------------
    # SECTION 1: EXECUTIVE SUMMARY
    # -------------------------------------------------------------
    h1 = doc.add_heading(level=1)
    r = h1.add_run("1. Executive Summary & Architecture Overview")
    r.font.size = Pt(16)
    r.font.color.rgb = RGBColor(0x1E, 0x1B, 0x4B)
    
    p = doc.add_paragraph(
        "This comprehensive documentation report constitutes the formal technical deliverable for the Field Training "
        "Course (Academic Year 2026-2027) at Al-Hikma University under the academic supervision of Prof. Ibrahim Ahmed Al-Baltah. "
        "The project was executed as a simulated individual Full-Stack Software Engineering endeavor, covering every tier "
        "of the Software Development Life Cycle (SDLC) from initial architectural planning and domain design to automated testing, "
        "multi-tier deployment, and documentation."
    )
    
    p = doc.add_paragraph(
        "The system delivers a modern, distributed hospitality reservation platform comprised of two decoupled architectural assemblies:"
    )
    
    p_b1 = doc.add_paragraph(style='List Bullet')
    r = p_b1.add_run("Core Backend & Administrative Web Portal (HotelBookingSystem): ")
    r.font.bold = True
    p_b1.add_run("Engineered using ASP.NET Core 10 following Clean Architecture and the Repository Pattern. It encapsulates the relational data layer, secure RESTful API endpoints, and a server-rendered administrative MVC web portal.")

    p_b2 = doc.add_paragraph(style='List Bullet')
    r = p_b2.add_run("Autonomous Mobile Client (HotelBookingSystem-Flutter): ")
    r.font.bold = True
    p_b2.add_run("Engineered using Google's Flutter framework with the Provider Pattern. It operates as an independent micro-client communicating exclusively via JSON-over-HTTP with the backend RESTful API.")

    # Architecture Diagram callout
    p_arch = doc.add_paragraph()
    set_cell_background_p = p_arch.paragraph_format
    p_arch.paragraph_format.space_before = Pt(8)
    p_arch.paragraph_format.space_after = Pt(8)
    r_arch = p_arch.add_run(
        "Architectural Layer Dependency Flow:\n"
        "  [ MVC Web Portal (Razor + DataTables) ]   [ Standalone Flutter Mobile Client ]\n"
        "                     │                                      │\n"
        "                     └───────────────┬──────────────────────┘\n"
        "                                     ▼\n"
        "                        [ RESTful Web API Controller ]\n"
        "                                     │\n"
        "                                     ▼\n"
        "                      [ Application Services & Contracts ]\n"
        "                                     │\n"
        "                                     ▼\n"
        "                          [ Domain Entities & Logic ]\n"
        "                                     ▲\n"
        "                                     │ Entity Framework Core 10\n"
        "                     [ Infrastructure Tier (SQL Server) ]"
    )
    r_arch.font.name = 'Consolas'
    r_arch.font.size = Pt(9)
    r_arch.font.color.rgb = RGBColor(0x1F, 0x29, 0x37)

    # -------------------------------------------------------------
    # SECTION 2: INSTALLATION & DEPLOYMENT GUIDE (W10.1)
    # -------------------------------------------------------------
    h1 = doc.add_heading(level=1)
    r = h1.add_run("2. Environmental Deployment and Installation Guide (W10.1)")
    r.font.size = Pt(16)
    r.font.color.rgb = RGBColor(0x1E, 0x1B, 0x4B)

    doc.add_paragraph(
        "This chapter provides definitive, verified instructions to install, configure, compile, and execute all "
        "subsystems from clean checkouts."
    )
    
    h2 = doc.add_heading(level=2)
    h2.add_run("2.1 Prerequisites").font.size = Pt(13)
    
    prereq_table = doc.add_table(rows=6, cols=3)
    prereq_table.alignment = WD_TABLE_ALIGNMENT.CENTER
    set_table_borders(prereq_table, "E5E7EB", "4")
    
    headers = ["Component", "Minimum Requirement", "Purpose"]
    for i, h in enumerate(headers):
        cell = prereq_table.rows[0].cells[i]
        set_cell_background(cell, "1E1B4B")
        p = cell.paragraphs[0]
        r = p.add_run(h)
        r.font.bold = True
        r.font.color.rgb = RGBColor(0xFF, 0xFF, 0xFF)
        r.font.size = Pt(9.5)
        
    prereqs = [
        (".NET SDK", ".NET 10.0 (x64)", "Compiling and running Web API and ASP.NET MVC"),
        ("Database Engine", "SQL Server LocalDB or SQL Server 2022", "Relational persistence tier for hotel and booking records"),
        ("Mobile SDK", "Flutter 3.19+ & Dart 3.x", "Building and running the independent cross-platform client"),
        ("Web Browser", "Google Chrome 120+", "Testing MVC portal, Swagger UI, and Flutter Web"),
        ("Version Control", "Git CLI 2.40+", "Managing branches, merges, and remote repositories")
    ]
    for row_idx, data in enumerate(prereqs, start=1):
        row = prereq_table.rows[row_idx]
        bg = "F9FAFB" if row_idx % 2 == 1 else "FFFFFF"
        for col_idx, text in enumerate(data):
            cell = row.cells[col_idx]
            set_cell_background(cell, bg)
            set_cell_margins(cell, 60, 60, 100, 100)
            p = cell.paragraphs[0]
            r = p.add_run(text)
            r.font.size = Pt(9.5)

    h2 = doc.add_heading(level=2)
    h2.add_run("2.2 Database Initialization and Automated Seeding").font.size = Pt(13)
    doc.add_paragraph(
        "The project enforces automated schema creation and data seeding. Upon startup of the Web API or MVC portal, "
        "the database is automatically verified via Database.EnsureCreated(), and populated with initial test data via "
        "DbSeeder.SeedAsync():"
    )
    p_b = doc.add_paragraph(style='List Bullet')
    p_b.add_run("12 Unique Luxury Hotels: ").font.bold = True
    p_b.add_run("Located in prominent cities across the Arab world, each with high-resolution photography.")
    
    p_b = doc.add_paragraph(style='List Bullet')
    p_b.add_run("48 Dedicated Room Records: ").font.bold = True
    p_b.add_run("4 rooms per hotel with diverse capacities (Single, Double, Suite), prices, and distinct room photography.")

    p_b = doc.add_paragraph(style='List Bullet')
    p_b.add_run("Pre-configured Credentials: ").font.bold = True
    p_b.add_run("Admin user: admin@hotel.com / Password123! (salted PBKDF2 hash).")

    h2 = doc.add_heading(level=2)
    h2.add_run("2.3 Step-by-Step Execution Commands").font.size = Pt(13)

    p_code = doc.add_paragraph()
    r = p_code.add_run(
        "# 1. Clone & Build the Core Backend Solution:\n"
        "git clone https://github.com/LoaiAlsorory/HotelBookingSystem.git\n"
        "cd HotelBookingSystem\n"
        "dotnet restore HotelBookingSystem.sln\n"
        "dotnet build HotelBookingSystem.sln --configuration Release\n\n"
        "# 2. Launch the Web API Service (Terminal 1):\n"
        "cd src/HotelBookingSystem.API\n"
        "dotnet run\n"
        "# -> API live on http://localhost:5242\n"
        "# -> Swagger UI live on http://localhost:5242/swagger\n\n"
        "# 3. Launch the Administrative Web Portal (Terminal 2):\n"
        "cd src/HotelBookingSystem.Web\n"
        "dotnet run\n"
        "# -> Web Portal live on http://localhost:5200\n\n"
        "# 4. Launch the Autonomous Flutter Mobile Client (Terminal 3):\n"
        "cd E:/Project_ASP.NET/HOTEL_BOOKIN_SYSTEM/Hotel_Booking_Flutter\n"
        "flutter pub get\n"
        "flutter run -d chrome   # Or 'flutter run' for connected Android device"
    )
    r.font.name = 'Consolas'
    r.font.size = Pt(9)
    r.font.color.rgb = RGBColor(0x1F, 0x29, 0x37)

    doc.add_page_break()

    # -------------------------------------------------------------
    # SECTION 3: USER MANUAL (W10.2)
    # -------------------------------------------------------------
    h1 = doc.add_heading(level=1)
    r = h1.add_run("3. Comprehensive User Manual and Screen Walkthrough (W10.2)")
    r.font.size = Pt(16)
    r.font.color.rgb = RGBColor(0x1E, 0x1B, 0x4B)

    doc.add_paragraph(
        "The system delivers dedicated interfaces for two principal audiences: the Administrative Staff "
        "operating through the desktop-optimized MVC Web Portal, and the Guests operating through the Flutter Mobile App."
    )

    h2 = doc.add_heading(level=2)
    h2.add_run("3.1 Administrative MVC Web Portal Guide").font.size = Pt(13)

    portal_steps = [
        ("Authentication (/Account/Login)", "Access the portal via modern split-screen login. Enter email and password. Validation alerts trigger for invalid credentials or malformed inputs."),
        ("Management Dashboard (/Home/Index)", "Presents executive metrics: total hotels, total rooms, active bookings, and gross revenue. Features quick-action navigation buttons and real-time recent bookings list."),
        ("Hotel Operations (/Hotels)", "View table indexed by DataTables.js with instant search. Add new hotel properties with name, city, address, rating, and image URL. Confirm deletions via asynchronous Bootstrap Modal."),
        ("Room Inventory (/Rooms)", "Manage rooms per hotel property. Configure room numbers, types (Single, Double, Suite), capacity (1-10 guests), nightly rates, and availability toggle states."),
        ("Booking Management (/Bookings)", "Inspect all system reservations. Create reservations with automated cost calculations (Nights * Rate). Modify booking status between Pending, Confirmed, and Cancelled."),
        ("User Directory (/Users)", "Administrative overview of registered accounts and associated system roles (Admin, Customer).")
    ]
    for title, desc in portal_steps:
        p = doc.add_paragraph()
        r_t = p.add_run(f"• {title}: ")
        r_t.font.bold = True
        r_t.font.color.rgb = RGBColor(0x4F, 0x46, 0xE5)
        p.add_run(desc)

    h2 = doc.add_heading(level=2)
    h2.add_run("3.2 Flutter Mobile Application Guide").font.size = Pt(13)

    mobile_steps = [
        ("Splash & Authentication Screen", "Animated brand greeting initializing token storage. Split login interface with password masking and validation."),
        ("Main Screen & Navigation Shell", "Features a native right-to-left (RTL) Drawer menu for profile and logout, a cohesive AppBar, and a persistent 5-item BottomNavigationBar."),
        ("Home Dashboard Screen", "Metric cards displaying live statistics, horizontal carousel of featured hotels, and quick reservation shortcuts."),
        ("Hotels Catalog Screen", "Instant text-search filter across hotel names and cities with photo cards and rating badges."),
        ("Hotel Details Screen", "High-resolution property showcase, address overview, description, and dynamic list of available rooms."),
        ("Interactive SheetModal Booking", "Clicking 'Book Now' opens an interactive modal bottom sheet allowing guests to select dates, guest counts, review computed totals, and confirm bookings with immediate API feedback.")
    ]
    for title, desc in mobile_steps:
        p = doc.add_paragraph()
        r_t = p.add_run(f"• {title}: ")
        r_t.font.bold = True
        r_t.font.color.rgb = RGBColor(0x7C, 0x3A, 0xED)
        p.add_run(desc)

    # -------------------------------------------------------------
    # SECTION 4: TESTING & QA (W10.3)
    # -------------------------------------------------------------
    h1 = doc.add_heading(level=1)
    r = h1.add_run("4. Testing and Quality Assurance Verification (W10.3)")
    r.font.size = Pt(16)
    r.font.color.rgb = RGBColor(0x1E, 0x1B, 0x4B)

    doc.add_paragraph(
        "A rigorous, multi-tiered testing methodology was executed to guarantee functional correctness, API reliability, "
        "input validation integrity, and business rule enforcement."
    )

    h2 = doc.add_heading(level=2)
    h2.add_run("4.1 RESTful API Test Results (Postman & Swagger)").font.size = Pt(13)

    test_table = doc.add_table(rows=9, cols=5)
    test_table.alignment = WD_TABLE_ALIGNMENT.CENTER
    set_table_borders(test_table, "E5E7EB", "4")

    cols = ["Endpoint", "Method", "Test Scenario", "Expected Status", "Verification"]
    for i, c in enumerate(cols):
        cell = test_table.rows[0].cells[i]
        set_cell_background(cell, "1E1B4B")
        p = cell.paragraphs[0]
        r = p.add_run(c)
        r.font.bold = True
        r.font.color.rgb = RGBColor(0xFF, 0xFF, 0xFF)
        r.font.size = Pt(9)

    tests = [
        ("/api/auth/login", "POST", "Valid administrator login", "200 OK", "Passed (JWT Token)"),
        ("/api/auth/login", "POST", "Invalid password attempt", "401 Unauthorized", "Passed (Error message)"),
        ("/api/hotels", "GET", "Fetch hotel catalog", "200 OK", "Passed (12 hotels)"),
        ("/api/hotels/99999", "GET", "Query non-existent ID", "404 Not Found", "Passed"),
        ("/api/hotels", "POST", "Create hotel (empty name)", "400 Bad Request", "Passed (Validation error)"),
        ("/api/rooms", "GET", "Filter rooms by hotelId", "200 OK", "Passed (4 rooms)"),
        ("/api/bookings", "POST", "Create valid reservation", "201 Created", "Passed (Record created)"),
        ("/api/bookings", "POST", "Double-booking same room", "409 Conflict", "Passed (Rejected)")
    ]
    for row_idx, data in enumerate(tests, start=1):
        row = test_table.rows[row_idx]
        bg = "F9FAFB" if row_idx % 2 == 1 else "FFFFFF"
        for col_idx, val in enumerate(data):
            cell = row.cells[col_idx]
            set_cell_background(cell, bg)
            set_cell_margins(cell, 50, 50, 80, 80)
            p = cell.paragraphs[0]
            r = p.add_run(val)
            r.font.size = Pt(8.5)
            if col_idx == 4:
                r.font.bold = True
                r.font.color.rgb = RGBColor(0x05, 0x96, 0x69) # Green

    h2 = doc.add_heading(level=2)
    h2.add_run("4.2 Input Validation Testing Matrix").font.size = Pt(13)

    val_table = doc.add_table(rows=6, cols=4)
    val_table.alignment = WD_TABLE_ALIGNMENT.CENTER
    set_table_borders(val_table, "E5E7EB", "4")

    cols = ["Entity / Attribute", "Validation Constraint", "Negative Test Payload", "System Handling"]
    for i, c in enumerate(cols):
        cell = val_table.rows[0].cells[i]
        set_cell_background(cell, "374151")
        p = cell.paragraphs[0]
        r = p.add_run(c)
        r.font.bold = True
        r.font.color.rgb = RGBColor(0xFF, 0xFF, 0xFF)
        r.font.size = Pt(9)

    val_data = [
        ("User.Email", "Required, EmailAddress", "admin_at_hotel", "Rejected: invalid email syntax"),
        ("Hotel.Rating", "Range (1.0 to 5.0)", "6.5", "Rejected: rating out of bounds"),
        ("Room.PricePerNight", "Range (1 to 1,000,000)", "-150.00", "Rejected: price must be positive"),
        ("Room.Capacity", "Range (1 to 20)", "0", "Rejected: capacity must be >= 1"),
        ("Booking.Dates", "IValidatableObject rule", "CheckOut <= CheckIn", "Rejected: checkout must be after checkin")
    ]
    for row_idx, data in enumerate(val_data, start=1):
        row = val_table.rows[row_idx]
        bg = "F9FAFB" if row_idx % 2 == 1 else "FFFFFF"
        for col_idx, val in enumerate(data):
            cell = row.cells[col_idx]
            set_cell_background(cell, bg)
            set_cell_margins(cell, 50, 50, 80, 80)
            p = cell.paragraphs[0]
            r = p.add_run(val)
            r.font.size = Pt(8.5)

    doc.add_page_break()

    # -------------------------------------------------------------
    # SECTION 5: SYNTHESIS OF W2 - W9
    # -------------------------------------------------------------
    h1 = doc.add_heading(level=1)
    r = h1.add_run("5. Cumulative Milestone Synthesis (W2 - W9 Alignment)")
    r.font.size = Pt(16)
    r.font.color.rgb = RGBColor(0x1E, 0x1B, 0x4B)

    milestones = [
        ("Week 2: Proposal & Requirements (5%)", "Formulated project scope, 12 functional requirements (FR1-FR12), and 6 non-functional requirements (security, availability, performance)."),
        ("Week 3: Software Design & Clean Architecture (15%)", "Designed Clean Architecture domain layers, Repository Pattern contracts, Use Case diagrams, Sequence models, and Class ERD models."),
        ("Week 4: UI/UX Prototyping (5%)", "Engineered high-fidelity interactive prototypes in Adobe XD (XD.xd) with brand tokens: Indigo (#4F46E5), Violet (#7C3AED), and Gold (#C9971F)."),
        ("Week 5: Web API Development (10%)", "Developed 18 secured RESTful endpoints with ASP.NET Core 10, JWT bearer authorization, and comprehensive input validation."),
        ("Week 6: Automated API Testing (5%)", "Created automated Postman collection suite and interactive Swagger/OpenAPI documentation."),
        ("Week 7: ASP.NET Core MVC Portal (25%)", "Full server-rendered web application with Bootstrap 5 RTL, Cairo typography, CSS variables, DataTables.js, and modal delete confirmations."),
        ("Week 8: Flutter Mobile Application (15%)", "Cross-platform mobile client featuring Provider state management, RTL Drawer, BottomNavigationBar, AppBar, SheetModal, and Cairo font."),
        ("Week 9: GitHub Source Control & Merging (5%)", "Applied Git branching model with main and feature branches, non-fast-forward merge commits, and decoupled independent repositories per supervisor instruction."),
        ("Week 10: Documentation & Technical Manual (15%)", "Installation guide, user manuals, testing verification, engineering conclusions, and academic bibliography.")
    ]
    for title, desc in milestones:
        p = doc.add_paragraph()
        r_t = p.add_run(f"• {title}: ")
        r_t.font.bold = True
        r_t.font.color.rgb = RGBColor(0x1F, 0x29, 0x37)
        p.add_run(desc)

    # -------------------------------------------------------------
    # SECTION 6: CONCLUSION & FUTURE WORK (W10.4)
    # -------------------------------------------------------------
    h1 = doc.add_heading(level=1)
    r = h1.add_run("6. Conclusion, Engineering Challenges, and Future Roadmap (W10.4)")
    r.font.size = Pt(16)
    r.font.color.rgb = RGBColor(0x1E, 0x1B, 0x4B)

    doc.add_paragraph(
        "The Field Training Course at Al-Hikma University provided a realistic simulation of enterprise-grade software development. "
        "The candidate successfully engineered a complete full-stack solution from requirements to release. "
        "Critical engineering challenges were encountered and methodically solved:"
    )

    challenges = [
        ("Standalone Client Decoupling", "Adhering to Prof. Ibrahim Al-Baltah's instruction, the Flutter mobile project was cleanly extracted into an autonomous directory and dedicated GitHub repository, establishing independent build and versioning lifecycles."),
        ("Temporal Synchronization & UTC Boundaries", "Resolved timezone offsets across clients by normalizing all date payloads to UTC (DateTime.SpecifyKind(d, DateTimeKind.Utc)) at the Application layer boundary."),
        ("Viewport Overflow Prevention", "Engineered a custom ResponsivePhoneFrame widget using LayoutBuilder to eliminate visual striping and overflow across variable mobile screen dimensions.")
    ]
    for c_title, c_desc in challenges:
        p = doc.add_paragraph()
        r_t = p.add_run(f"• {c_title}: ")
        r_t.font.bold = True
        p.add_run(c_desc)

    doc.add_paragraph("Future development roadmap highlights:")
    doc.add_paragraph(style='List Bullet').add_run("Payment Gateway Integration (Stripe, PayPal, and local bank settlement APIs).")
    doc.add_paragraph(style='List Bullet').add_run("Cloud Push Notifications via Firebase Cloud Messaging (FCM).")
    doc.add_paragraph(style='List Bullet').add_run("AI-Powered Room Recommendation Engine utilizing collaborative filtering.")

    # -------------------------------------------------------------
    # SECTION 7: REFERENCES (W10.5)
    # -------------------------------------------------------------
    h1 = doc.add_heading(level=1)
    r = h1.add_run("7. Academic and Technical References (W10.5)")
    r.font.size = Pt(16)
    r.font.color.rgb = RGBColor(0x1E, 0x1B, 0x4B)

    refs = [
        "Martin, Robert C. (Uncle Bob) (2017). Clean Architecture: A Craftsman's Guide to Software Structure and Design. Prentice Hall.",
        "Microsoft Corporation (2025/2026). ASP.NET Core 10.0 Architecture and Web API Guidance. Microsoft Learn Documentation.",
        "Entity Framework Core Team (2025). Entity Framework Core: Code-First Modeling and Concurrency Control. Microsoft Learn.",
        "Google LLC (2024/2026). Flutter Framework Documentation: State Management via Provider Pattern. Flutter.dev.",
        "Fielding, Roy Thomas (2000). Architectural Styles and the Design of Network-based Software Architectures. Doctoral dissertation, UC Irvine.",
        "Internet Engineering Task Force (IETF) (2015). RFC 7519: JSON Web Token (JWT). IETF Standards.",
        "Al-Hikma University (2026). Field Training Course Guide: Comprehensive Simulation Syllabus for Individual Full-Stack Software Development Project. Department of Information Technology, Sana'a, Yemen."
    ]
    for idx, ref in enumerate(refs, start=1):
        p = doc.add_paragraph()
        p.paragraph_format.left_indent = Inches(0.25)
        p.paragraph_format.first_line_indent = Inches(-0.25)
        r = p.add_run(f"[{idx}] ")
        r.font.bold = True
        p.add_run(ref)

    # Save output
    output_dir = os.path.dirname(__file__)
    output_path = os.path.join(output_dir, "Hotel_Booking_System_W10_Documentation.docx")
    doc.save(output_path)
    print("Document successfully created at:", output_path)

if __name__ == "__main__":
    create_document()
