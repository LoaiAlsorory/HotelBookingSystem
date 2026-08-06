using HotelBookingSystem.Domain.Entities;

namespace HotelBookingSystem.Application.Interfaces;

public interface IHotelRepository : IGenericRepository<Hotel>
{
    Task<IEnumerable<Hotel>> SearchByCityAsync(string city);
}
