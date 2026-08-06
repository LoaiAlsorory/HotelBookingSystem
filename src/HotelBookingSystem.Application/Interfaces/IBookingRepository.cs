using HotelBookingSystem.Domain.Entities;

namespace HotelBookingSystem.Application.Interfaces;

public interface IBookingRepository : IGenericRepository<Booking>
{
    Task<IEnumerable<Booking>> GetByUserIdAsync(int userId);

    // يتحقق من وجود تعارض حجز (Double Booking) لنفس الغرفة في فترة متداخلة - FR4
    Task<bool> HasConflictAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null);
}
