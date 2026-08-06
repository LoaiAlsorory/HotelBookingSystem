using HotelBookingSystem.Application.Interfaces;
using HotelBookingSystem.Domain.Entities;
using HotelBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email) =>
        await DbSet.FirstOrDefaultAsync(u => u.Email == email);
}
