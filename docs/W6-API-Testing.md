# W6 — اختبار وتوثيق Web API

## أدوات الاختبار
- **Swagger UI**: مدمج في المشروع، متاح تلقائيًا على المسار الجذر عند تشغيل `HotelBookingSystem.API`.
- **Postman**: مجموعة طلبات جاهزة في `HotelBookingSystem.postman_collection.json` (مرفقة في نفس المجلد).

## طريقة التشغيل
1. افتح `HotelBookingSystem.sln`، اجعل `HotelBookingSystem.API` مشروع بدء التشغيل (Startup Project).
2. شغّل المشروع (F5) — ستفتح واجهة Swagger تلقائيًا وتظهر عنوان القاعدة (Base URL)، مثل: `https://localhost:7000`.
3. افتح Postman، استورد ملف `HotelBookingSystem.postman_collection.json` (File → Import).
4. عدّل متغيّر `baseUrl` في تبويب Variables ليطابق العنوان الذي ظهر في المتصفح.
5. نفّذ الطلبات بالترتيب: Auth أولًا (لإنشاء بيانات/تسجيل دخول)، ثم Hotels، ثم Rooms، ثم Bookings، ثم Users.

## نتائج الاختبار

| # | Endpoint | Method | حالة الاختبار | النتيجة المتوقعة | النتيجة الفعلية | الحالة |
|---|---|---|---|---|---|---|
| 1 | /api/auth/register | POST | بيانات صحيحة | 200/201 + Token | | ⬜ |
| 2 | /api/auth/register | POST | بريد مكرر | 409 Conflict | | ⬜ |
| 3 | /api/auth/register | POST | بيانات ناقصة/بريد غير صالح | 400 Bad Request | | ⬜ |
| 4 | /api/auth/login | POST | بيانات صحيحة | 200 + Token | | ⬜ |
| 5 | /api/auth/login | POST | كلمة مرور خاطئة | 401 Unauthorized | | ⬜ |
| 6 | /api/hotels | GET | عرض الكل | 200 + مصفوفة | | ⬜ |
| 7 | /api/hotels/{id} | GET | معرف موجود | 200 | | ⬜ |
| 8 | /api/hotels/{id} | GET | معرف غير موجود | 404 Not Found | | ⬜ |
| 9 | /api/hotels | POST | بيانات صحيحة | 201 Created | | ⬜ |
| 10 | /api/hotels | POST | اسم فارغ / تقييم خارج المدى | 400 Bad Request | | ⬜ |
| 11 | /api/hotels/{id} | PUT | تحديث صحيح | 204 No Content | | ⬜ |
| 12 | /api/hotels/{id} | DELETE | فندق مرتبط بغرف | 409 Conflict | | ⬜ |
| 13 | /api/rooms?hotelId= | GET | فلترة حسب الفندق | 200 | | ⬜ |
| 14 | /api/rooms | POST | بيانات صحيحة | 201 Created | | ⬜ |
| 15 | /api/rooms | POST | hotelId غير موجود | 404 Not Found | | ⬜ |
| 16 | /api/rooms/{id} | PUT | تحديث صحيح | 204 No Content | | ⬜ |
| 17 | /api/rooms/{id} | DELETE | غرفة مرتبطة بحجوزات | 409 Conflict | | ⬜ |
| 18 | /api/bookings | POST | بيانات صحيحة | 201 Created | | ⬜ |
| 19 | /api/bookings | POST | تاريخ خروج قبل الدخول | 400 Bad Request | | ⬜ |
| 20 | /api/bookings | POST | تعارض حجز لنفس الغرفة/الفترة | 409 Conflict | | ⬜ |
| 21 | /api/bookings | POST | عدد ضيوف يتجاوز السعة | 400 Bad Request | | ⬜ |
| 22 | /api/bookings?userId= | GET | فلترة حسب المستخدم | 200 | | ⬜ |
| 23 | /api/bookings/{id}/status | PUT | تعديل الحالة | 204 No Content | | ⬜ |
| 24 | /api/bookings/{id} | DELETE | حذف صحيح | 204 No Content | | ⬜ |
| 25 | /api/users | GET | عرض الكل | 200 | | ⬜ |
| 26 | /api/users | POST | بريد مكرر | 409 Conflict | | ⬜ |
| 27 | /api/users/{id} | GET | معرف غير موجود | 404 Not Found | | ⬜ |

> عبّئ عمودي "النتيجة الفعلية" و"الحالة" (✅ Pass / ❌ Fail) بعد تنفيذ كل طلب في Postman، وأرفق لقطة شاشة لكل صف في مجلد `screenshots/`.

## الحماية (Authorization)
تم تطبيق `[Authorize]` على متحكمات: `HotelsController`, `RoomsController`, `BookingsController`, `UsersController`. فقط `AuthController` (Register/Login) متاح بدون تسجيل دخول (كما يجب). لذلك عند اختبار أي Endpoint غير Auth بدون إرسال Token، يجب أن ترجع النتيجة **401 Unauthorized**.

عند استخدام مجموعة Postman المرفقة، بعد تنفيذ طلب **Login - نجاح** يُحفظ الـ Token تلقائيًا في متغيّر `{{token}}` ويُستخدم تلقائيًا في كل الطلبات التالية (لأن الـ Authorization معرّف على مستوى الـ Collection بالكامل). لذلك نفّذ دائمًا طلب Login بنجاح أولًا قبل تجربة أي طلب آخر محمي.

أضف حالة اختبار إضافية يدويًا في الجدول أعلاه لكل Controller محمي: تنفيذ الطلب **بدون** Token → يجب أن يرجع 401.

## Screenshots
ضع لقطات الشاشة هنا بالتسلسل:
- `screenshots/01-register-success.png`
- `screenshots/02-login-success.png`
- `screenshots/03-hotel-create-success.png`
- ...
