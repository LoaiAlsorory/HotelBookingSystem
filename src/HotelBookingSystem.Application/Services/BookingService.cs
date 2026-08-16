using HotelBookingSystem.Application.Common;
using HotelBookingSystem.Application.DTOs.Booking;
using HotelBookingSystem.Application.Interfaces;
using HotelBookingSystem.Domain.Entities;

namespace HotelBookingSystem.Application.Services;

// ينفّذ منطق عملية "حجز غرفة" الموثّق في مخطط التسلسل (Sequence Diagram) بوثيقة W3:
// يتحقق من وجود الغرفة، ثم يستدعي BookingRepository للتحقق من التعارض (FR4)
// قبل حفظ الحجز فعلياً وحساب السعر الإجمالي.
public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IRoomRepository _roomRepo;
    private readonly IUserRepository _userRepo;

    public BookingService(IBookingRepository bookingRepo, IRoomRepository roomRepo, IUserRepository userRepo)
    {
        _bookingRepo = bookingRepo;
        _roomRepo = roomRepo;
        _userRepo = userRepo;
    }

    public async Task<IEnumerable<BookingDto>> GetAllAsync()
    {
        var bookings = await _bookingRepo.GetAllAsync();
        return bookings.Select(ToDto);
    }

    public async Task<IEnumerable<BookingDto>> GetByUserIdAsync(int userId)
    {
        var bookings = await _bookingRepo.GetByUserIdAsync(userId);
        return bookings.Select(ToDto);
    }

    public async Task<BookingDto?> GetByIdAsync(int id)
    {
        var booking = await _bookingRepo.GetByIdAsync(id);
        return booking is null ? null : ToDto(booking);
    }

    public async Task<Result<BookingDto>> CreateAsync(CreateBookingDto dto)
    {
        // 1. التحقق من وجود المستخدم
        var userExists = await _userRepo.GetByIdAsync(dto.UserId);
        if (userExists is null)
            return Result<BookingDto>.NotFound($"المستخدم رقم {dto.UserId} غير موجود");

        // 2. التحقق من وجود الغرفة
        var room = await _roomRepo.GetByIdAsync(dto.RoomId);
        if (room is null)
            return Result<BookingDto>.NotFound($"الغرفة رقم {dto.RoomId} غير موجودة");

        if (!room.IsAvailable)
            return Result<BookingDto>.BadRequest("هذه الغرفة غير متاحة للحجز حالياً");

        // 3. التحقق المنطقي من سعة الغرفة (Business Rule: GuestsCount <= Capacity)
        if (dto.GuestsCount > room.Capacity)
            return Result<BookingDto>.BadRequest($"عدد الضيوف ({dto.GuestsCount}) يتجاوز سعة الغرفة القصوى ({room.Capacity} أفراد)");

        // 4. منع تعارض الحجوزات على نفس الغرفة لنفس الفترة (FR4 - Double Booking)
        var hasConflict = await _bookingRepo.HasConflictAsync(dto.RoomId, dto.CheckInDate, dto.CheckOutDate);
        if (hasConflict)
            return Result<BookingDto>.Conflict("الغرفة محجوزة بالفعل في هذه الفترة الزمنية");

        var nights = (dto.CheckOutDate.Date - dto.CheckInDate.Date).Days;
        if (nights <= 0) nights = 1;

        var totalPrice = room.PricePerNight * nights;

        var booking = new Booking
        {
            UserId = dto.UserId,
            RoomId = dto.RoomId,
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            GuestsCount = dto.GuestsCount,
            TotalPrice = totalPrice
        };

        var created = await _bookingRepo.AddAsync(booking);
        return Result<BookingDto>.Ok(ToDto(created));
    }


    public async Task<Result<BookingDto>> UpdateAsync(int id, UpdateBookingDto dto)
    {
        var booking = await _bookingRepo.GetByIdAsync(id);
        if (booking is null)
            return Result<BookingDto>.NotFound($"الحجز رقم {id} غير موجود");

        // 1. التحقق من وجود الغرفة الجديدة (قد تكون تغيّرت)
        var room = await _roomRepo.GetByIdAsync(dto.RoomId);
        if (room is null)
            return Result<BookingDto>.NotFound($"الغرفة رقم {dto.RoomId} غير موجودة");

        // 2. سعة الغرفة
        if (dto.GuestsCount > room.Capacity)
            return Result<BookingDto>.BadRequest($"عدد الضيوف ({dto.GuestsCount}) يتجاوز سعة الغرفة القصوى ({room.Capacity} أفراد)");

        // 3. منع تعارض الحجوزات مع أي حجز آخر لنفس الغرفة لنفس الفترة (باستثناء هذا الحجز نفسه)
        var hasConflict = await _bookingRepo.HasConflictAsync(dto.RoomId, dto.CheckInDate, dto.CheckOutDate, id);
        if (hasConflict)
            return Result<BookingDto>.Conflict("الغرفة محجوزة بالفعل في هذه الفترة الزمنية");

        var nights = (dto.CheckOutDate.Date - dto.CheckInDate.Date).Days;
        if (nights <= 0) nights = 1;

        booking.RoomId = dto.RoomId;
        booking.CheckInDate = dto.CheckInDate;
        booking.CheckOutDate = dto.CheckOutDate;
        booking.GuestsCount = dto.GuestsCount;
        booking.Status = dto.Status;
        booking.TotalPrice = room.PricePerNight * nights;

        await _bookingRepo.UpdateAsync(booking);

        // إعادة التحميل مع بيانات الغرفة/الفندق المحدّثة لعرضها بشكل صحيح فورًا
        var refreshed = await _bookingRepo.GetByIdAsync(id);
        return Result<BookingDto>.Ok(ToDto(refreshed!));
    }

    public async Task<bool> UpdateStatusAsync(int id, UpdateBookingStatusDto dto)
    {
        var booking = await _bookingRepo.GetByIdAsync(id);
        if (booking is null) return false;

        booking.Status = dto.Status;
        await _bookingRepo.UpdateAsync(booking);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var booking = await _bookingRepo.GetByIdAsync(id);
        if (booking is null) return false;

        await _bookingRepo.DeleteAsync(booking);
        return true;
    }

    private static BookingDto ToDto(Booking b) => new()
    {
        Id = b.Id,
        UserId = b.UserId,
        UserName = b.User?.FullName ?? string.Empty,
        RoomId = b.RoomId,
        RoomNumber = b.Room?.RoomNumber ?? string.Empty,
        HotelName = b.Room?.Hotel?.Name ?? string.Empty,
        CheckInDate = b.CheckInDate,
        CheckOutDate = b.CheckOutDate,
        GuestsCount = b.GuestsCount,
        TotalPrice = b.TotalPrice,
        Status = b.Status.ToString(),
        CreatedAt = b.CreatedAt
    };
}
