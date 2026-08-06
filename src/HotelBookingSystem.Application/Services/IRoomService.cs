using HotelBookingSystem.Application.DTOs.Room;

namespace HotelBookingSystem.Application.Services;

public interface IRoomService
{
    Task<IEnumerable<RoomDto>> GetAllAsync();
    Task<IEnumerable<RoomDto>> GetByHotelIdAsync(int hotelId);
    Task<RoomDto?> GetByIdAsync(int id);
    Task<RoomDto> CreateAsync(CreateRoomDto dto);
    Task<bool> UpdateAsync(int id, UpdateRoomDto dto);
    Task<bool> DeleteAsync(int id);
}
