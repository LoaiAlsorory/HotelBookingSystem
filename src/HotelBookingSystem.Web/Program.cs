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
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
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
            // نفس روابط الصور الأصلية التي كانت تعمل بنجاح في المشروع، نعيد استخدامها
            // على كل الفنادق/الغرف الجديدة لضمان عدم كسر عرض الصور مطلقًا.
            const string imgHotelA = "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=600";
            const string imgHotelB = imgHotelA; // نفس الصورة الأصلية المؤكد عملها؛ تفاديًا لأي خطر كسر عرض الصور
            const string imgRoomA = "https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=600";
            const string imgRoomB = "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=600";
            const string imgRoomC = "https://images.unsplash.com/photo-1582719508461-905c673771fd?w=600";

            var hotel1 = new Hotel
            {
                Name = "فندق الحكمة الدولي",
                City = "صنعاء",
                Address = "شارع الزبيري، صنعاء",
                Description = "فندق فاخر خمس نجوم في قلب العاصمة صنعاء، يجمع بين الضيافة اليمنية الأصيلة والرفاهية العصرية.",
                ImageUrl = imgHotelA,
                Rating = 4.8
            };
            var hotel2 = new Hotel
            {
                Name = "منتجع البحر الأحمر",
                City = "الحديدة",
                Address = "الكورنيش، الحديدة",
                Description = "منتجع ساحلي مطل مباشرة على البحر الأحمر، بشواطئ خاصة وأجواء استوائية هادئة.",
                ImageUrl = imgHotelB,
                Rating = 4.5
            };
            var hotel3 = new Hotel
            {
                Name = "فندق قصر عدن",
                City = "عدن",
                Address = "خليج التواهي، عدن",
                Description = "فندق تاريخي مطلّ على الميناء، يمزج بين العمارة الاستعمارية الأنيقة وخدمات الفنادق الحديثة.",
                ImageUrl = imgHotelA,
                Rating = 4.3
            };
            var hotel4 = new Hotel
            {
                Name = "منتجع تعز الجبلي",
                City = "تعز",
                Address = "سفح جبل صبر، تعز",
                Description = "استراحة جبلية هادئة بإطلالات خلابة، مثالية للاسترخاء بعيدًا عن صخب المدينة.",
                ImageUrl = imgHotelB,
                Rating = 4.1
            };
            var hotel5 = new Hotel
            {
                Name = "فندق الخليج الذهبي",
                City = "دبي",
                Address = "نخلة جميرا، دبي",
                Description = "تجربة إقامة استثنائية بلمسات ذهبية فاخرة وإطلالات بانورامية على الخليج العربي.",
                ImageUrl = imgHotelA,
                Rating = 4.9
            };
            var hotel6 = new Hotel
            {
                Name = "فندق النخيل الملكي",
                City = "الرياض",
                Address = "حي العليا، الرياض",
                Description = "فندق أعمال وترفيه راقٍ في قلب العاصمة، قريب من أبرز المراكز التجارية.",
                ImageUrl = imgHotelB,
                Rating = 4.6
            };

            db.Hotels.AddRange(hotel1, hotel2, hotel3, hotel4, hotel5, hotel6);
            await db.SaveChangesAsync();

            db.Rooms.AddRange(
                // فندق الحكمة الدولي - صنعاء
                new Room { HotelId = hotel1.Id, RoomNumber = "101", RoomType = "مفردة", PricePerNight = 25000, Capacity = 1, IsAvailable = true, ImageUrl = imgRoomA },
                new Room { HotelId = hotel1.Id, RoomNumber = "102", RoomType = "مزدوجة", PricePerNight = 40000, Capacity = 2, IsAvailable = true, ImageUrl = imgRoomB },
                new Room { HotelId = hotel1.Id, RoomNumber = "103", RoomType = "جناح ملكي", PricePerNight = 95000, Capacity = 4, IsAvailable = true, ImageUrl = imgRoomC },

                // منتجع البحر الأحمر - الحديدة
                new Room { HotelId = hotel2.Id, RoomNumber = "201", RoomType = "جناح", PricePerNight = 70000, Capacity = 4, IsAvailable = true, ImageUrl = imgRoomC },
                new Room { HotelId = hotel2.Id, RoomNumber = "202", RoomType = "مزدوجة إطلالة بحرية", PricePerNight = 55000, Capacity = 2, IsAvailable = true, ImageUrl = imgRoomB },
                new Room { HotelId = hotel2.Id, RoomNumber = "203", RoomType = "مفردة", PricePerNight = 22000, Capacity = 1, IsAvailable = false, ImageUrl = imgRoomA },

                // فندق قصر عدن
                new Room { HotelId = hotel3.Id, RoomNumber = "301", RoomType = "مفردة", PricePerNight = 20000, Capacity = 1, IsAvailable = true, ImageUrl = imgRoomA },
                new Room { HotelId = hotel3.Id, RoomNumber = "302", RoomType = "مزدوجة", PricePerNight = 35000, Capacity = 2, IsAvailable = true, ImageUrl = imgRoomB },

                // منتجع تعز الجبلي
                new Room { HotelId = hotel4.Id, RoomNumber = "401", RoomType = "شاليه عائلي", PricePerNight = 48000, Capacity = 5, IsAvailable = true, ImageUrl = imgRoomC },
                new Room { HotelId = hotel4.Id, RoomNumber = "402", RoomType = "مزدوجة", PricePerNight = 30000, Capacity = 2, IsAvailable = true, ImageUrl = imgRoomB },

                // فندق الخليج الذهبي - دبي
                new Room { HotelId = hotel5.Id, RoomNumber = "501", RoomType = "جناح فاخر", PricePerNight = 150000, Capacity = 3, IsAvailable = true, ImageUrl = imgRoomC },
                new Room { HotelId = hotel5.Id, RoomNumber = "502", RoomType = "مزدوجة ديلوكس", PricePerNight = 90000, Capacity = 2, IsAvailable = true, ImageUrl = imgRoomB },
                new Room { HotelId = hotel5.Id, RoomNumber = "503", RoomType = "بنتهاوس", PricePerNight = 260000, Capacity = 6, IsAvailable = true, ImageUrl = imgRoomA },

                // فندق النخيل الملكي - الرياض
                new Room { HotelId = hotel6.Id, RoomNumber = "601", RoomType = "غرفة أعمال", PricePerNight = 60000, Capacity = 1, IsAvailable = true, ImageUrl = imgRoomA },
                new Room { HotelId = hotel6.Id, RoomNumber = "602", RoomType = "جناح تنفيذي", PricePerNight = 110000, Capacity = 3, IsAvailable = true, ImageUrl = imgRoomC }
            );
            await db.SaveChangesAsync();
        }
    }
}
