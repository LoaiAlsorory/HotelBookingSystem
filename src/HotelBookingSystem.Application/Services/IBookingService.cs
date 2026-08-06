using HotelBookingSystem.Application.Common;
using HotelBookingSystem.Application.DTOs.Booking;

namespace HotelBookingSystem.Application.Services;

public interface IBookingService
{
    Task<IEnumerable<BookingDto>> GetAllAsync();
    Task<IEnumerable<BookingDto>> GetByUserIdAsync(int userId);
    Task<BookingDto?> GetByIdAsync(int id);
    Task<Result<BookingDto>> CreateAsync(CreateBookingDto dto);
    Task<bool> UpdateStatusAsync(int id, UpdateBookingStatusDto dto);
    Task<bool> DeleteAsync(int id);
}
