using HotelBookingSystem.Application.Interfaces;
using HotelBookingSystem.Domain.Entities;
using HotelBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Infrastructure.Repositories;

public class HotelRepository : GenericRepository<Hotel>, IHotelRepository
{
    public HotelRepository(AppDbContext context) : base(context) { }

    public override async Task<IEnumerable<Hotel>> GetAllAsync() =>
        await DbSet.Include(h => h.Rooms).ToListAsync();

    public override async Task<Hotel?> GetByIdAsync(int id) =>
        await DbSet.Include(h => h.Rooms).FirstOrDefaultAsync(h => h.Id == id);

    public async Task<IEnumerable<Hotel>> SearchByCityAsync(string city) =>
        await DbSet.Include(h => h.Rooms)
                   .Where(h => h.City.Contains(city))
                   .ToListAsync();
}
