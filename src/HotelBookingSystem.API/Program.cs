using System.Text;
using HotelBookingSystem.API.Middleware;
using HotelBookingSystem.Infrastructure;
using HotelBookingSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ---------- Services ----------
builder.Services.AddControllers();

// إعداد المصادقة باستخدام JWT Bearer Tokens
var secretKey = builder.Configuration["JwtSettings:Secret"] ?? "HotelBookingSystemSecretKey_FieldTraining_W5_2026_SecureKey_AlHikmaUniversity";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "HotelBookingSystemAPI",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtSettings:Audience"] ?? "HotelBookingSystemClient",
        ValidateLifetime = true
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hotel Booking System API",
        Version = "v1",
        Description = "Web API لنظام حجز الفنادق - جامعة الحكمة - Field Training W5/W6"
    });

    // إمكانية تجربة الـ Token مباشرة من واجهة Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "أدخل رمز JWT بالمخطط التالي: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// طبقة Infrastructure (DbContext + Repositories + Services) - Clean Architecture
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// ---------- إنشاء قاعدة البيانات إن لم تكن موجودة ----------
// ملاحظة: البذر الفعلي للبيانات التجريبية يتم من مشروع Web (Program.cs) عند تشغيله،
// لكن هذا السطر يضمن عدم فشل الـ API لو تم تشغيله بمفرده قبل تشغيل الـ Web مرة واحدة على الأقل.
// ملاحظة إضافية: عند تشغيل API وWeb معًا (Multiple Startup Projects) قد يحاول كلاهما
// إنشاء نفس القاعدة في نفس اللحظة (Race Condition)، فيفشل أحدهما بخطأ SQL "Database already
// exists" رغم أن هذا يعني فعليًا أن الهدف (وجود القاعدة) قد تحقق. لذلك نلتقط هذا الخطأ تحديدًا
// ونتجاهله بأمان دون التأثير على أي منطق آخر.
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
}

// ---------- Middleware Pipeline ----------
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel Booking System API v1");
        options.RoutePrefix = string.Empty; // Swagger على المسار الجذر مباشرة
    });
}

// app.UseHttpsRedirection(); // Commented out to allow HTTP requests from Flutter Web (solves SSL cert/CORS issues)
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

