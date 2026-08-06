using HotelBookingSystem.Application.Interfaces;
using HotelBookingSystem.Domain.Entities;
using HotelBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Infrastructure.Repositories;

public class BookingRepository : GenericRepository<Booking>, IBookingRepository
{
    public BookingRepository(AppDbContext context) : base(context) { }

    public override async Task<IEnumerable<Booking>> GetAllAsync() =>
        await DbSet.Include(b => b.User)
                   .Include(b => b.Room).ThenInclude(r => r!.Hotel)
                   .ToListAsync();

    public override async Task<Booking?> GetByIdAsync(int id) =>
        await DbSet.Include(b => b.User)
                   .Include(b => b.Room).ThenInclude(r => r!.Hotel)
                   .FirstOrDefaultAsync(b => b.Id == id);

    public async Task<IEnumerable<Booking>> GetByUserIdAsync(int userId) =>
        await DbSet.Include(b => b.Room).ThenInclude(r => r!.Hotel)
                   .Where(b => b.UserId == userId)
                   .ToListAsync();

    public async Task<bool> HasConflictAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null)
    {
        // تعارض الفترات: يتحقق أن الحجز الجديد لا يتقاطع زمنياً مع أي حجز
        // قائم (غير ملغي) لنفس الغرفة - تطبيق مباشر لـ FR4
        var query = DbSet.Where(b =>
            b.RoomId == roomId &&
            b.Status != Domain.Enums.BookingStatus.Cancelled &&
            checkIn < b.CheckOutDate &&
            checkOut > b.CheckInDate);

        if (excludeBookingId.HasValue)
            query = query.Where(b => b.Id != excludeBookingId.Value);

        return await query.AnyAsync();
    }
}
