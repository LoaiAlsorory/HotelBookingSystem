using HotelBookingSystem.Application.Interfaces;
using HotelBookingSystem.Application.Services;
using HotelBookingSystem.Infrastructure.Persistence;
using HotelBookingSystem.Infrastructure.Repositories;
using HotelBookingSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBookingSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Token Service
        services.AddScoped<ITokenService, JwtTokenService>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IHotelRepository, HotelRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();

        // Application Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IHotelService, HotelService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IBookingService, BookingService>();

        return services;
    }
}

