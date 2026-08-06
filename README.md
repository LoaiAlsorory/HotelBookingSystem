# Hotel Booking System — Web API (W5)

مشروع Web API مبني بـ **ASP.NET Core 10** وفق **Clean Architecture + Repository Pattern**
(كما هو محدد في وثيقة التصميم W3)، يغطي متطلبات الأسبوع الخامس:
هيكلة المشروع (Project Structure)، نقاط النهاية (Endpoints)، والتحقق من صحة المدخلات (Validation).

## هيكلة المشروع (Clean Architecture)

```
HotelBookingSystem/
├── src/
│   ├── HotelBookingSystem.Domain          → الكيانات الأساسية (User, Hotel, Room, Booking)
│   ├── HotelBookingSystem.Application     → DTOs + واجهات Repository + منطق الأعمال (Services)
│   ├── HotelBookingSystem.Infrastructure  → DbContext (EF Core) + تنفيذ Repositories
│   └── HotelBookingSystem.API             → Controllers + Program.cs + Swagger
└── HotelBookingSystem.sln
```

اتجاه الاعتمادية: `API → Infrastructure → Application → Domain` (كما في مخطط W3).

## الكيانات والعلاقات

- **Hotel — Room**: 1 إلى عدد (فندق واحد له عدة غرف)
- **User — Booking**: 1 إلى عدد (مستخدم واحد له عدة حجوزات)
- **Room — Booking**: 1 إلى عدد (غرفة واحدة تُحجز في فترات متعددة غير متعارضة)

## تشغيل المشروع محلياً

1. تأكد من تثبيت [.NET SDK](https://dotnet.microsoft.com/download) و SQL Server (أو LocalDB).
2. من مجلد الجذر:
   ```bash
   dotnet restore
   ```
3. عدّل Connection String في `src/HotelBookingSystem.API/appsettings.json` إذا لزم.
4. أنشئ قاعدة البيانات (من مجلد API):
   ```bash
   cd src/HotelBookingSystem.API
   dotnet tool install --global dotnet-ef   # مرة واحدة فقط إذا لم يكن مثبتاً
   dotnet ef migrations add InitialCreate --project ../HotelBookingSystem.Infrastructure --startup-project .
   dotnet ef database update --project ../HotelBookingSystem.Infrastructure --startup-project .
   ```
5. تشغيل المشروع:
   ```bash
   dotnet run
   ```
6. سيفتح Swagger UI تلقائياً على المسار الجذر (`https://localhost:xxxx/`) — وهذا هو أساس مهمة W6 لاحقاً.

## نقاط النهاية (Endpoints)

| الكيان | GET (الكل) | GET (واحد) | POST | PUT | DELETE |
|---|---|---|---|---|---|
| Users | `/api/users` | `/api/users/{id}` | `/api/users` | `/api/users/{id}` | `/api/users/{id}` |
| Hotels | `/api/hotels` | `/api/hotels/{id}` | `/api/hotels` | `/api/hotels/{id}` | `/api/hotels/{id}` |
| Rooms | `/api/rooms` (أو `?hotelId=`) | `/api/rooms/{id}` | `/api/rooms` | `/api/rooms/{id}` | `/api/rooms/{id}` |
| Bookings | `/api/bookings` (أو `?userId=`) | `/api/bookings/{id}` | `/api/bookings` | `/api/bookings/{id}/status` | `/api/bookings/{id}` |

## Validation والأمان المطبّق

- حقول مطلوبة (`[Required]`) على كل الحقول الأساسية
- التحقق من صيغة البريد الإلكتروني (`[EmailAddress]`)
- التحقق من الطول (`[StringLength]`) والنطاق الرقمي (`[Range]`)
- تحقق مخصص (`IValidatableObject`) في `CreateBookingDto`: تاريخ الخروج بعد تاريخ الدخول، وعدم قبول تواريخ ماضية
- التحقق من سعة الغرفة القصوى (`GuestsCount <= Room.Capacity`)
- منع تعارض الحجوزات (Double Booking) — منطق عمل في `BookingService`/`BookingRepository` يطبّق FR4 من وثيقة W2
- تشفير كلمات المرور باستخدام PBKDF2 مع Salt عشوائي مخصص 16-Byte لضمان أعلى أمان هجومي
- توثيق ومصادقة إلكترونية عبر JWT Tokens وتوفير تجربة الإرسال والـ Authorize مباشرة في Swagger

## الأسبوع السادس (W6) — دليل الاختبار السريع

المشروع جاهز تماماً للاختبار عبر Swagger / Postman:
1. **POST /api/auth/register** أو **POST /api/auth/login**: الحصول على الـ Token.
2. ضغط زر **Authorize** أفي أعلى صفحة Swagger وإدخال `Bearer {token}`.
3. تجربة عمليات Add / Update / Delete / Search / Book واختبار الحالات النجاح والأخطاء المتوقعة (400, 404, 409).

---

## الأسبوع السابع (W7) — تطبيق ASP.NET Core MVC

مشروع `HotelBookingSystem.Web` يوفّر واجهة ويب كاملة (Server-Side MVC) تستهلك نفس طبقات
Application/Infrastructure مباشرة (بدون المرور عبر الـ API) وتغطي جميع متطلبات دليل التدريب الميداني:

- **لوحة تحكم رئيسية (Main Dashboard)**: `/` — إحصائيات مباشرة (عدد المستخدمين، الفنادق، الغرف،
  الحجوزات، إجمالي الإيرادات) + قائمة أحدث الحجوزات + عرض الفنادق المسجّلة (تُستخدم أيضًا كصفحة
  ويب رئيسية لعرض المعلومات).
- **CRUD كامل لكل كيان** (Users, Hotels, Rooms, Bookings): Index / Details / Create / Edit / Delete
  مع Input Validation (Required، أطوال، بريد إلكتروني، Range، إلخ) على مستوى Client و Server.
- **ثلاث جداول مترابطة على الأقل**: Hotel → Room → Booking ← User (4 جداول فعليًا).
- **جدول به صور**: Hotel و Room كلاهما يحتوي حقل `ImageUrl` معروض في القوائم والتفاصيل.
- **Bootstrap 5 (RTL) + Bootstrap Icons + خط Cairo مخصص + متغيرات CSS (`:root { --hbs-* }`)
  + Bootstrap Modal (تأكيد الحذف الموحّد) + DataTables.js** (بحث/فرز/ترقيم صفحات على كل الجداول).

### تشغيل مشروع الويب (W7) محليًا

1. تأكد من تثبيت [.NET SDK 10](https://dotnet.microsoft.com/download) وقاعدة بيانات SQL Server
   (أو SQL Server LocalDB المرفق مع Visual Studio على Windows).
2. عدّل Connection String إذا لزم في `src/HotelBookingSystem.Web/appsettings.json`
   (الافتراضي يعمل مباشرة مع LocalDB على Windows).
3. لا حاجة لتنفيذ أوامر Migrations يدويًا لتشغيل هذا المشروع: عند أول تشغيل، السطر
   `db.Database.EnsureCreated()` في `Program.cs` يُنشئ قاعدة البيانات وجداولها تلقائيًا،
   ثم `DbSeeder.SeedAsync` يزرع بيانات تجريبية (مستخدمَين، فندقين، 3 غرف) لتجربة الموقع فورًا.
4. من مجلد المشروع:
   ```bash
   cd src/HotelBookingSystem.Web
   dotnet restore
   dotnet run
   ```
5. افتح المتصفح على الرابط الذي يظهر في الطرفية (مثل `https://localhost:7050`).
6. بيانات تجريبية جاهزة للاطّلاع: مستخدم إداري `admin@hbs.com` وفندقان بهما 3 غرف — يمكنك مباشرة
   تجربة الإضافة / التعديل / الحذف / الحجز من الواجهة.

> **ملاحظة مهمة**: مشروعا `Web` و`API` يستخدمان نفس قاعدة البيانات (`HotelBookingSystemDb`) عبر نفس
> طبقة Infrastructure. يمكنك تشغيل أي منهما بشكل مستقل؛ كلاهما يضمن إنشاء الجداول تلقائيًا عند أول
> تشغيل، لكن التشغيل الأول لمشروع Web هو الذي يزرع البيانات التجريبية.

### GitHub — الفروع والدمج (متطلب رقم 7)

سير العمل الموصى به لهذا المشروع:

```bash
git init -b main
git add .
git commit -m "Initial commit: Clean Architecture solution (Domain/Application/Infrastructure/API/Web)"
git remote add origin <رابط-المستودع-على-GitHub>
git push -u origin main

# لكل ميزة أو أسبوع جديد: افتح فرعًا منفصلاً
git checkout -b feature/w7-mvc-web
# ... عمل التعديلات ...
git add .
git commit -m "feat: ASP.NET Core MVC Web app (W7) - CRUD, Dashboard, Bootstrap, DataTables"
git push -u origin feature/w7-mvc-web

# ثم افتح Pull Request على GitHub من feature/w7-mvc-web إلى main وادمجه (Merge)،
# أو محليًا:
git checkout main
git merge feature/w7-mvc-web
git push origin main
```

كرّر هذا النمط لكل أسبوع/مرحلة (مثال: `feature/w5-webapi`، `feature/w8-flutter`،
`feature/w9-docs`) بحيث يظهر في المستودع أكثر من فرع وسجل Merge واضح يوثّق تطور المشروع.

