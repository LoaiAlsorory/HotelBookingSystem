using HotelBookingSystem.Domain.Entities;

namespace HotelBookingSystem.Application.Interfaces;

public interface IRoomRepository : IGenericRepository<Room>
{
    Task<IEnumerable<Room>> GetByHotelIdAsync(int hotelId);
}
