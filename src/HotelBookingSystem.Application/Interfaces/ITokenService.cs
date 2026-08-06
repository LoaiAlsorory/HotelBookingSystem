using HotelBookingSystem.Domain.Entities;

namespace HotelBookingSystem.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
