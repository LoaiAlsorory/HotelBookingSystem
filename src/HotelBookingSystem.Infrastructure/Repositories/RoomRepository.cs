using HotelBookingSystem.Application.Interfaces;
using HotelBookingSystem.Domain.Entities;
using HotelBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Infrastructure.Repositories;

public class RoomRepository : GenericRepository<Room>, IRoomRepository
{
    public RoomRepository(AppDbContext context) : base(context) { }

    public override async Task<IEnumerable<Room>> GetAllAsync() =>
        await DbSet.Include(r => r.Hotel).ToListAsync();

    public override async Task<Room?> GetByIdAsync(int id) =>
        await DbSet.Include(r => r.Hotel).FirstOrDefaultAsync(r => r.Id == id);

    public async Task<IEnumerable<Room>> GetByHotelIdAsync(int hotelId) =>
        await DbSet.Include(r => r.Hotel)
                   .Where(r => r.HotelId == hotelId)
                   .ToListAsync();
}
