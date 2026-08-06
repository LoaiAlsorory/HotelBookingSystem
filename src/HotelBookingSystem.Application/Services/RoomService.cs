using HotelBookingSystem.Application.DTOs.Room;
using HotelBookingSystem.Application.Interfaces;
using HotelBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Application.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepo;
    private readonly IHotelRepository _hotelRepo;

    public RoomService(IRoomRepository roomRepo, IHotelRepository hotelRepo)
    {
        _roomRepo = roomRepo;
        _hotelRepo = hotelRepo;
    }

    public async Task<IEnumerable<RoomDto>> GetAllAsync()
    {
        var rooms = await _roomRepo.GetAllAsync();
        return rooms.Select(ToDto);
    }

    public async Task<IEnumerable<RoomDto>> GetByHotelIdAsync(int hotelId)
    {
        var rooms = await _roomRepo.GetByHotelIdAsync(hotelId);
        return rooms.Select(ToDto);
    }

    public async Task<RoomDto?> GetByIdAsync(int id)
    {
        var room = await _roomRepo.GetByIdAsync(id);
        return room is null ? null : ToDto(room);
    }

    public async Task<RoomDto> CreateAsync(CreateRoomDto dto)
    {
        var hotelExists = await _hotelRepo.ExistsAsync(dto.HotelId);
        if (!hotelExists)
            throw new InvalidOperationException("الفندق المحدد غير موجود");

        var room = new Room
        {
            HotelId = dto.HotelId,
            RoomNumber = dto.RoomNumber,
            RoomType = dto.RoomType,
            PricePerNight = dto.PricePerNight,
            Capacity = dto.Capacity,
            ImageUrl = dto.ImageUrl,
            IsAvailable = dto.IsAvailable
        };

        var created = await _roomRepo.AddAsync(room);
        return ToDto(created);
    }

    public async Task<bool> UpdateAsync(int id, UpdateRoomDto dto)
    {
        var room = await _roomRepo.GetByIdAsync(id);
        if (room is null) return false;

        room.RoomNumber = dto.RoomNumber;
        room.RoomType = dto.RoomType;
        room.PricePerNight = dto.PricePerNight;
        room.Capacity = dto.Capacity;
        room.ImageUrl = dto.ImageUrl;
        room.IsAvailable = dto.IsAvailable;

        await _roomRepo.UpdateAsync(room);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var room = await _roomRepo.GetByIdAsync(id);
        if (room is null) return false;

        try
        {
            await _roomRepo.DeleteAsync(room);
            return true;
        }
        catch (DbUpdateException)
        {
            // القيد Restrict على Room->Booking يمنع الحذف إذا كانت هناك حجوزات مرتبطة
            throw new InvalidOperationException("لا يمكن حذف هذه الغرفة لوجود حجوزات مرتبطة بها. يمكنك حذف الحجوزات أولًا أو تعطيل توفر الغرفة بدلًا من ذلك.");
        }
    }

    private static RoomDto ToDto(Room r) => new()
    {
        Id = r.Id,
        HotelId = r.HotelId,
        HotelName = r.Hotel?.Name ?? string.Empty,
        RoomNumber = r.RoomNumber,
        RoomType = r.RoomType,
        PricePerNight = r.PricePerNight,
        Capacity = r.Capacity,
        ImageUrl = r.ImageUrl,
        IsAvailable = r.IsAvailable
    };
}
