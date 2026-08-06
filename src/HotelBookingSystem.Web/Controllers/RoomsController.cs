using HotelBookingSystem.Application.DTOs.Room;
using HotelBookingSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelBookingSystem.Web.Controllers;

public class RoomsController : Controller
{
    private readonly IRoomService _roomService;
    private readonly IHotelService _hotelService;

    public RoomsController(IRoomService roomService, IHotelService hotelService)
    {
        _roomService = roomService;
        _hotelService = hotelService;
    }

    private async Task PopulateHotelsDropdown(int? selectedHotelId = null)
    {
        var hotels = await _hotelService.GetAllAsync();
        ViewBag.Hotels = new SelectList(hotels, "Id", "Name", selectedHotelId);
    }

    // GET: /Rooms
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "الغرف";
        var rooms = await _roomService.GetAllAsync();
        return View(rooms);
    }

    // GET: /Rooms/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var room = await _roomService.GetByIdAsync(id);
        if (room is null) return NotFound();

        ViewData["Title"] = "تفاصيل الغرفة";
        return View(room);
    }

    // GET: /Rooms/Create
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "إضافة غرفة";
        await PopulateHotelsDropdown();
        return View();
    }

    // POST: /Rooms/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRoomDto dto)
    {
        ViewData["Title"] = "إضافة غرفة";
        if (!ModelState.IsValid)
        {
            await PopulateHotelsDropdown(dto.HotelId);
            return View(dto);
        }

        try
        {
            await _roomService.CreateAsync(dto);
            TempData["Success"] = "تمت إضافة الغرفة بنجاح";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await PopulateHotelsDropdown(dto.HotelId);
            return View(dto);
        }
    }

    // GET: /Rooms/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var room = await _roomService.GetByIdAsync(id);
        if (room is null) return NotFound();

        var dto = new UpdateRoomDto
        {
            RoomNumber = room.RoomNumber,
            RoomType = room.RoomType,
            PricePerNight = room.PricePerNight,
            Capacity = room.Capacity,
            ImageUrl = room.ImageUrl,
            IsAvailable = room.IsAvailable
        };

        ViewData["Title"] = "تعديل غرفة";
        ViewBag.RoomId = id;
        ViewBag.HotelName = room.HotelName;
        return View(dto);
    }

    // POST: /Rooms/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateRoomDto dto)
    {
        ViewData["Title"] = "تعديل غرفة";
        if (!ModelState.IsValid)
        {
            ViewBag.RoomId = id;
            var existing = await _roomService.GetByIdAsync(id);
            ViewBag.HotelName = existing?.HotelName;
            return View(dto);
        }

        var updated = await _roomService.UpdateAsync(id, dto);
        if (!updated) return NotFound();

        TempData["Success"] = "تم تعديل بيانات الغرفة بنجاح";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Rooms/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var room = await _roomService.GetByIdAsync(id);
        if (room is null) return NotFound();

        ViewData["Title"] = "حذف غرفة";
        return View(room);
    }

    // POST: /Rooms/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var deleted = await _roomService.DeleteAsync(id);
            TempData[deleted ? "Success" : "Error"] = deleted
                ? "تم حذف الغرفة بنجاح"
                : "تعذّر حذف الغرفة، قد تكون غير موجودة";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }
}
