using HotelBookingSystem.Application.Common;
using HotelBookingSystem.Application.DTOs.Auth;
using HotelBookingSystem.Application.DTOs.User;

namespace HotelBookingSystem.Application.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(int id);
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task<bool> UpdateAsync(int id, UpdateUserDto dto);
    Task<bool> DeleteAsync(int id);
    Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto);
    Task<Result<AuthResponseDto>> AuthenticateAsync(LoginDto dto);
}

