using HotelBookingSystem.Application.Services;
using HotelBookingSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.Web.Controllers;

public class HomeController : Controller
{
    private readonly IUserService _userService;
    private readonly IHotelService _hotelService;
    private readonly IRoomService _roomService;
    private readonly IBookingService _bookingService;

    public HomeController(
        IUserService userService,
        IHotelService hotelService,
        IRoomService roomService,
        IBookingService bookingService)
    {
        _userService = userService;
        _hotelService = hotelService;
        _roomService = roomService;
        _bookingService = bookingService;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllAsync();
        var hotels = await _hotelService.GetAllAsync();
        var rooms = await _roomService.GetAllAsync();
        var bookings = await _bookingService.GetAllAsync();

        var vm = new DashboardViewModel
        {
            UsersCount = users.Count(),
            HotelsCount = hotels.Count(),
            RoomsCount = rooms.Count(),
            BookingsCount = bookings.Count(),
            TotalRevenue = bookings.Where(b => b.Status != "Cancelled").Sum(b => b.TotalPrice),
            Hotels = hotels.ToList(),
            RecentBookings = bookings.OrderByDescending(b => b.CreatedAt).Take(5).ToList()
        };

        ViewData["Title"] = "لوحة التحكم";
        return View(vm);
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
    }
}
