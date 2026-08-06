using HotelBookingSystem.Application.DTOs.Hotel;

namespace HotelBookingSystem.Application.Services;

public interface IHotelService
{
    Task<IEnumerable<HotelDto>> GetAllAsync();
    Task<HotelDto?> GetByIdAsync(int id);
    Task<HotelDto> CreateAsync(CreateHotelDto dto);
    Task<bool> UpdateAsync(int id, UpdateHotelDto dto);
    Task<bool> DeleteAsync(int id);
}
