using HotelBookingSystem.Domain.Entities;

namespace HotelBookingSystem.Application.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}
