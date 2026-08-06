using HotelBookingSystem.Application.DTOs.Hotel;
using HotelBookingSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.Web.Controllers;

public class HotelsController : Controller
{
    private readonly IHotelService _hotelService;
    private readonly IRoomService _roomService;

    public HotelsController(IHotelService hotelService, IRoomService roomService)
    {
        _hotelService = hotelService;
        _roomService = roomService;
    }

    // GET: /Hotels
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "الفنادق";
        var hotels = await _hotelService.GetAllAsync();
        return View(hotels);
    }

    // GET: /Hotels/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var hotel = await _hotelService.GetByIdAsync(id);
        if (hotel is null) return NotFound();

        ViewBag.Rooms = await _roomService.GetByHotelIdAsync(id);
        ViewData["Title"] = "تفاصيل الفندق";
        return View(hotel);
    }

    // GET: /Hotels/Create
    public IActionResult Create()
    {
        ViewData["Title"] = "إضافة فندق";
        return View();
    }

    // POST: /Hotels/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateHotelDto dto)
    {
        ViewData["Title"] = "إضافة فندق";
        if (!ModelState.IsValid) return View(dto);

        await _hotelService.CreateAsync(dto);
        TempData["Success"] = "تمت إضافة الفندق بنجاح";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Hotels/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var hotel = await _hotelService.GetByIdAsync(id);
        if (hotel is null) return NotFound();

        var dto = new UpdateHotelDto
        {
            Name = hotel.Name,
            City = hotel.City,
            Address = hotel.Address,
            Description = hotel.Description,
            ImageUrl = hotel.ImageUrl,
            Rating = hotel.Rating
        };

        ViewData["Title"] = "تعديل فندق";
        ViewBag.HotelId = id;
        return View(dto);
    }

    // POST: /Hotels/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateHotelDto dto)
    {
        ViewData["Title"] = "تعديل فندق";
        if (!ModelState.IsValid)
        {
            ViewBag.HotelId = id;
            return View(dto);
        }

        var updated = await _hotelService.UpdateAsync(id, dto);
        if (!updated) return NotFound();

        TempData["Success"] = "تم تعديل بيانات الفندق بنجاح";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Hotels/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var hotel = await _hotelService.GetByIdAsync(id);
        if (hotel is null) return NotFound();

        ViewData["Title"] = "حذف فندق";
        return View(hotel);
    }

    // POST: /Hotels/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var deleted = await _hotelService.DeleteAsync(id);
            TempData[deleted ? "Success" : "Error"] = deleted
                ? "تم حذف الفندق بنجاح"
                : "تعذّر حذف الفندق، قد يكون غير موجود";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }
}
