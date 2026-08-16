using System.Globalization;
using HotelBookingSystem.Domain.Entities;
using HotelBookingSystem.Domain.Enums;
using HotelBookingSystem.Infrastructure;
using HotelBookingSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// إجبار التطبيق على استخدام التقويم الميلادي (Gregorian) دائمًا بغض النظر عن ثقافة نظام التشغيل،
// لتفادي عرض التواريخ بالتقويم الهجري تلقائيًا في بعض بيئات Windows العربية.
// (ملاحظة: هذا وحده لا يكفي دائمًا لأن Kestrel/IIS قد يعيد استخدام Thread Pool threads
// تكوّنت ثقافتها من نظام التشغيل قبل هذا السطر؛ لذلك أضفنا أيضًا Middleware صريح أدناه
// يفرض الثقافة على كل طلب HTTP بشكل مؤكد، بالإضافة إلى InvariantCulture الصريحة في الـ Views)
var defaultCulture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en-US")
    .AddSupportedCultures("en-US")
    .AddSupportedUICultures("en-US");

// ---------- Services ----------
// كل الصفحات محمية تلقائياً (تتطلب تسجيل دخول) ما عدا ما يُعلَّم بـ [AllowAnonymous]
// (يُستخدم على AccountController فقط لصفحتي Login/AccessDenied)
builder.Services.AddControllersWithViews(options =>
{
    var requireAuthPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(requireAuthPolicy));
});

// تسجيل الدخول بالكوكيز (Cookie Authentication) لواجهة الويب - مستقل عن JWT الخاص بالـ API
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = "HotelBookingSystem.Web.Auth";
    });

// طبقة Infrastructure (DbContext + Repositories + Services) - نفس قاعدة بيانات W5/W6
// نستدعي الخدمات من Application/Infrastructure مباشرة (Clean Architecture) بدون HTTP وسيط
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// ---------- إنشاء/تحديث قاعدة البيانات وزرع بيانات تجريبية ----------
// ملاحظة: عند تشغيل Web وAPI معًا (Multiple Startup Projects) قد يحاول كلاهما إنشاء نفس
// القاعدة في نفس اللحظة (Race Condition)، فيفشل أحدهما بخطأ SQL "Database already exists"
// رغم أن هذا يعني فعليًا أن الهدف (وجود القاعدة) قد تحقق. لذلك نلتقط هذا الخطأ تحديدًا ونتجاهله بأمان.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.EnsureCreated();
    }
    catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Message.Contains("already exists"))
    {
        // القاعدة أصبحت موجودة بالفعل (أنشأها مشروع آخر يعمل بالتوازي في نفس اللحظة) — لا خطأ فعلي هنا.
    }
    await DbSeeder.SeedAsync(db);
}

// ---------- Middleware Pipeline ----------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// يفرض ثقافة en-US على كل طلب HTTP قادم بشكل مؤكد (يجب أن يكون من أوائل عناصر الـ Pipeline)
app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// يجب أن يسبق UseAuthentication كل من UseRouting ويسبق UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// ---------- بيانات تجريبية أولية ----------
public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (!await db.Users.AnyAsync())
        {
            db.Users.AddRange(
                new User
                {
                    FullName = "مدير النظام",
                    Email = "admin@hbs.com",
                    PasswordHash = HotelBookingSystem.Application.Services.UserService.HashPassword("Admin@123"),
                    PhoneNumber = "777000111",
                    Role = UserRole.Administrator
                },
                new User
                {
                    FullName = "أحمد سالم",
                    Email = "ahmed@hbs.com",
                    PasswordHash = HotelBookingSystem.Application.Services.UserService.HashPassword("User@123"),
                    PhoneNumber = "777123456",
                    Role = UserRole.RegisteredUser
                }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Hotels.AnyAsync())
        {
            // مجموعة صور فنادق/غرف من Unsplash (روابط دائمة عبر معرّف الصورة — لا تعتمد على اسم مستخدم
            // قد يتغيّر، وهي نفس النمط المستخدم أصلًا في المشروع). نكرر الاستخدام عبر عدة فنادق لضمان التنوّع
            // البصري مع بقاء كل رابط من نفس المجموعة "الآمنة" لتفادي أي صورة مكسورة.
            var hotelImages = new[]
            {
                "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800",
                "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800",
                "https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800",
                "https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?w=800",
                "https://images.unsplash.com/photo-1445019980597-93fa8acb246c?w=800",
                "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=800",
                "https://images.unsplash.com/photo-1618773928121-c32242e63f39?w=800",
                "https://images.unsplash.com/photo-1601565415267-724bbc5b3f3d?w=800",
            };
            var roomImages = new[]
            {
                "https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=700",
                "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=700",
                "https://images.unsplash.com/photo-1582719508461-905c673771fd?w=700",
                "https://images.unsplash.com/photo-1564501049412-61c2a3083791?w=700",
                "https://images.unsplash.com/photo-1590490360182-c33d57733427?w=700",
                "https://images.unsplash.com/photo-1571003123894-1f0594d2b5d9?w=700",
            };

            var hotels = new List<Hotel>
            {
                new() { Name = "فندق الحكمة الدولي",   City = "صنعاء",   Address = "شارع الزبيري، صنعاء",        Description = "فندق فاخر خمس نجوم في قلب العاصمة صنعاء، يجمع بين الضيافة اليمنية الأصيلة والرفاهية العصرية.", ImageUrl = hotelImages[0], Rating = 4.8 },
                new() { Name = "منتجع البحر الأحمر",    City = "الحديدة",  Address = "الكورنيش، الحديدة",          Description = "منتجع ساحلي مطل مباشرة على البحر الأحمر، بشواطئ خاصة وأجواء استوائية هادئة.",              ImageUrl = hotelImages[1], Rating = 4.5 },
                new() { Name = "فندق قصر عدن",         City = "عدن",      Address = "خليج التواهي، عدن",          Description = "فندق تاريخي مطلّ على الميناء، يمزج بين العمارة الاستعمارية الأنيقة وخدمات الفنادق الحديثة.",  ImageUrl = hotelImages[2], Rating = 4.3 },
                new() { Name = "منتجع تعز الجبلي",     City = "تعز",       Address = "سفح جبل صبر، تعز",           Description = "استراحة جبلية هادئة بإطلالات خلابة، مثالية للاسترخاء بعيدًا عن صخب المدينة.",                ImageUrl = hotelImages[3], Rating = 4.1 },
                new() { Name = "فندق الخليج الذهبي",    City = "دبي",       Address = "نخلة جميرا، دبي",            Description = "تجربة إقامة استثنائية بلمسات ذهبية فاخرة وإطلالات بانورامية على الخليج العربي.",             ImageUrl = hotelImages[4], Rating = 4.9 },
                new() { Name = "فندق النخيل الملكي",    City = "الرياض",    Address = "حي العليا، الرياض",          Description = "فندق أعمال وترفيه راقٍ في قلب العاصمة، قريب من أبرز المراكز التجارية.",                     ImageUrl = hotelImages[5], Rating = 4.6 },
                new() { Name = "فندق لؤلؤة جدة",       City = "جدة",       Address = "كورنيش جدة",                Description = "إطلالة ساحرة على البحر الأحمر مع شاطئ خاص ومسابح لا نهائية ومطاعم عالمية.",                  ImageUrl = hotelImages[6], Rating = 4.7 },
                new() { Name = "فندق أبراج الدوحة",     City = "الدوحة",    Address = "منطقة اللؤلؤة، الدوحة",      Description = "برج فندقي عصري بتصميم معماري مذهل وخدمات كونسيرج فاخرة على مدار الساعة.",                    ImageUrl = hotelImages[7], Rating = 4.4 },
                new() { Name = "فندق النيل الذهبي",     City = "القاهرة",   Address = "كورنيش النيل، القاهرة",      Description = "إقامة كلاسيكية فاخرة بإطلالة مباشرة على نهر النيل وقرب من أبرز المعالم التاريخية.",           ImageUrl = hotelImages[0], Rating = 4.2 },
                new() { Name = "منتجع مسقط الفيروزي",   City = "مسقط",      Address = "خليج العرب، مسقط",           Description = "منتجع هادئ بين الجبال والبحر، مثالي لعطلة استرخاء عائلية بمرافق سبا متكاملة.",               ImageUrl = hotelImages[1], Rating = 4.6 },
                new() { Name = "فندق برج الكويت",       City = "الكويت",    Address = "شارع الخليج العربي، الكويت", Description = "فندق أعمال حديث في قلب العاصمة، بقاعات مؤتمرات مجهزة وصالات تنفيذية راقية.",                 ImageUrl = hotelImages[2], Rating = 4.0 },
                new() { Name = "فندق أرز بيروت",        City = "بيروت",     Address = "وسط البلد، بيروت",           Description = "سحر المتوسط وأصالة الضيافة اللبنانية في فندق بوتيك أنيق قرب أشهر الأسواق والمطاعم.",         ImageUrl = hotelImages[3], Rating = 4.5 },
            };

            db.Hotels.AddRange(hotels);
            await db.SaveChangesAsync();

            var roomTypes = new (string Type, decimal Base, int Cap)[]
            {
                ("مفردة كلاسيكية",      22000, 1),
                ("مزدوجة قياسية",       38000, 2),
                ("جناح ديلوكس",         75000, 3),
                ("جناح عائلي",          58000, 5),
                ("جناح تنفيذي",         98000, 3),
                ("بنتهاوس فاخر",        190000, 6),
            };

            var rnd = new Random(42); // بذرة ثابتة لضمان نفس النتائج في كل تشغيل
            var rooms = new List<Room>();

            foreach (var hotel in hotels)
            {
                // كل فندق يحصل على 4 غرف متنوعة الأنواع والأسعار والصور
                var picks = roomTypes.OrderBy(_ => rnd.Next()).Take(4).ToList();
                var roomNoBase = (hotel.Id % 9 + 1) * 100;
                for (int i = 0; i < picks.Count; i++)
                {
                    var (type, basePrice, cap) = picks[i];
                    var priceJitter = 1 + (rnd.Next(-10, 16) / 100.0); // تفاوت طفيف بالسعر بين الفنادق
                    rooms.Add(new Room
                    {
                        HotelId = hotel.Id,
                        RoomNumber = (roomNoBase + i + 1).ToString(),
                        RoomType = type,
                        PricePerNight = Math.Round(basePrice * (decimal)priceJitter / 500m) * 500m,
                        Capacity = cap,
                        IsAvailable = rnd.Next(0, 10) > 1, // معظم الغرف متاحة، القليل محجوز لواقعية أكبر
                        ImageUrl = roomImages[rnd.Next(roomImages.Length)]
                    });
                }
            }

            db.Rooms.AddRange(rooms);
            await db.SaveChangesAsync();
        }
    }
}
