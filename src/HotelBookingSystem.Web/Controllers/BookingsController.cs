using HotelBookingSystem.Application.DTOs.Booking;
using HotelBookingSystem.Application.Services;
using HotelBookingSystem.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelBookingSystem.Web.Controllers;

public class BookingsController : Controller
{
    private readonly IBookingService _bookingService;
    private readonly IUserService _userService;
    private readonly IRoomService _roomService;

    public BookingsController(IBookingService bookingService, IUserService userService, IRoomService roomService)
    {
        _bookingService = bookingService;
        _userService = userService;
        _roomService = roomService;
    }

    private async Task PopulateDropdowns(int? selectedUserId = null, int? selectedRoomId = null)
    {
        var users = await _userService.GetAllAsync();
        var rooms = await _roomService.GetAllAsync();

        ViewBag.Users = new SelectList(users, "Id", "FullName", selectedUserId);
        ViewBag.Rooms = new SelectList(
            rooms.Select(r => new { r.Id, Display = $"{r.HotelName} - غرفة {r.RoomNumber} ({r.RoomType})" }),
            "Id", "Display", selectedRoomId);
    }

    // GET: /Bookings
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "الحجوزات";
        var bookings = await _bookingService.GetAllAsync();
        return View(bookings);
    }

    // GET: /Bookings/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var booking = await _bookingService.GetByIdAsync(id);
        if (booking is null) return NotFound();

        ViewData["Title"] = "تفاصيل الحجز";
        return View(booking);
    }

    // GET: /Bookings/Create
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "إضافة حجز";
        await PopulateDropdowns();
        return View(new CreateBookingDto { CheckInDate = DateTime.Today, CheckOutDate = DateTime.Today.AddDays(1), GuestsCount = 1 });
    }

    // POST: /Bookings/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBookingDto dto)
    {
        ViewData["Title"] = "إضافة حجز";
        if (!ModelState.IsValid)
        {
            await PopulateDropdowns(dto.UserId, dto.RoomId);
            return View(dto);
        }

        var result = await _bookingService.CreateAsync(dto);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "تعذّر إنشاء الحجز");
            await PopulateDropdowns(dto.UserId, dto.RoomId);
            return View(dto);
        }

        TempData["Success"] = "تم إنشاء الحجز بنجاح";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Bookings/Edit/5  (تعديل الحالة فقط)
    public async Task<IActionResult> Edit(int id)
    {
        var booking = await _bookingService.GetByIdAsync(id);
        if (booking is null) return NotFound();

        ViewData["Title"] = "تعديل حالة الحجز";
        ViewBag.Booking = booking;
        ViewBag.StatusList = new SelectList(Enum.GetValues(typeof(BookingStatus)));

        var currentStatus = Enum.TryParse<BookingStatus>(booking.Status, out var st) ? st : BookingStatus.Pending;
        return View(new UpdateBookingStatusDto { Status = currentStatus });
    }

    // POST: /Bookings/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateBookingStatusDto dto)
    {
        ViewData["Title"] = "تعديل حالة الحجز";
        if (!ModelState.IsValid)
        {
            ViewBag.Booking = await _bookingService.GetByIdAsync(id);
            ViewBag.StatusList = new SelectList(Enum.GetValues(typeof(BookingStatus)));
            return View(dto);
        }

        var updated = await _bookingService.UpdateStatusAsync(id, dto);
        if (!updated) return NotFound();

        TempData["Success"] = "تم تحديث حالة الحجز بنجاح";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Bookings/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var booking = await _bookingService.GetByIdAsync(id);
        if (booking is null) return NotFound();

        ViewData["Title"] = "حذف حجز";
        return View(booking);
    }

    // POST: /Bookings/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await _bookingService.DeleteAsync(id);
        if (!deleted)
        {
            TempData["Error"] = "تعذّر حذف الحجز، قد يكون غير موجود";
        }
        else
        {
            TempData["Success"] = "تم حذف الحجز بنجاح";
        }
        return RedirectToAction(nameof(Index));
    }
}
