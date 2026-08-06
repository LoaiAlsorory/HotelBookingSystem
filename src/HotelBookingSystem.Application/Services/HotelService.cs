using HotelBookingSystem.Application.DTOs.Hotel;
using HotelBookingSystem.Application.Interfaces;
using HotelBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Application.Services;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _repo;

    public HotelService(IHotelRepository repo) => _repo = repo;

    public async Task<IEnumerable<HotelDto>> GetAllAsync()
    {
        var hotels = await _repo.GetAllAsync();
        return hotels.Select(ToDto);
    }

    public async Task<HotelDto?> GetByIdAsync(int id)
    {
        var hotel = await _repo.GetByIdAsync(id);
        return hotel is null ? null : ToDto(hotel);
    }

    public async Task<HotelDto> CreateAsync(CreateHotelDto dto)
    {
        var hotel = new Hotel
        {
            Name = dto.Name,
            City = dto.City,
            Address = dto.Address,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            Rating = dto.Rating
        };

        var created = await _repo.AddAsync(hotel);
        return ToDto(created);
    }

    public async Task<bool> UpdateAsync(int id, UpdateHotelDto dto)
    {
        var hotel = await _repo.GetByIdAsync(id);
        if (hotel is null) return false;

        hotel.Name = dto.Name;
        hotel.City = dto.City;
        hotel.Address = dto.Address;
        hotel.Description = dto.Description;
        hotel.ImageUrl = dto.ImageUrl;
        hotel.Rating = dto.Rating;

        await _repo.UpdateAsync(hotel);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var hotel = await _repo.GetByIdAsync(id);
        if (hotel is null) return false;

        try
        {
            await _repo.DeleteAsync(hotel);
            return true;
        }
        catch (DbUpdateException)
        {
            // الحذف Cascade على Hotel->Room، لكن Room->Booking مقيّد بـ Restrict،
            // فإذا كانت إحدى غرف الفندق مرتبطة بحجوزات سيفشل الحذف على مستوى قاعدة البيانات
            throw new InvalidOperationException("لا يمكن حذف هذا الفندق لأن إحدى غرفه مرتبطة بحجوزات قائمة. يجب حذف تلك الحجوزات أولًا.");
        }
    }

    private static HotelDto ToDto(Hotel h) => new()
    {
        Id = h.Id,
        Name = h.Name,
        City = h.City,
        Address = h.Address,
        Description = h.Description,
        ImageUrl = h.ImageUrl,
        Rating = h.Rating,
        RoomsCount = h.Rooms?.Count ?? 0
    };
}
