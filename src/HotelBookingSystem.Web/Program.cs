using HotelBookingSystem.Domain.Entities;
using HotelBookingSystem.Domain.Enums;
using HotelBookingSystem.Infrastructure;
using HotelBookingSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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
            var hotel1 = new Hotel
            {
                Name = "فندق الحكمة الدولي",
                City = "صنعاء",
                Address = "شارع الزبيري، صنعاء",
                Description = "فندق فاخر في قلب العاصمة صنعاء",
                ImageUrl = "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=600",
                Rating = 4.5
            };
            var hotel2 = new Hotel
            {
                Name = "منتجع البحر الأحمر",
                City = "الحديدة",
                Address = "الكورنيش، الحديدة",
                Description = "منتجع ساحلي مطل على البحر الأحمر",
                ImageUrl = "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=600",
                Rating = 4.2
            };
            db.Hotels.AddRange(hotel1, hotel2);
            await db.SaveChangesAsync();

            db.Rooms.AddRange(
                new Room { HotelId = hotel1.Id, RoomNumber = "101", RoomType = "مفردة", PricePerNight = 25000, Capacity = 1, IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=600" },
                new Room { HotelId = hotel1.Id, RoomNumber = "102", RoomType = "مزدوجة", PricePerNight = 40000, Capacity = 2, IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=600" },
                new Room { HotelId = hotel2.Id, RoomNumber = "201", RoomType = "جناح", PricePerNight = 70000, Capacity = 4, IsAvailable = true, ImageUrl = "https://images.unsplash.com/photo-1582719508461-905c673771fd?w=600" }
            );
            await db.SaveChangesAsync();
        }
    }
}
