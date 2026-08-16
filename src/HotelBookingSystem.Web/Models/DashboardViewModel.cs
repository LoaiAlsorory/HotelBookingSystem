using HotelBookingSystem.Application.DTOs.Booking;
using HotelBookingSystem.Application.DTOs.Hotel;

namespace HotelBookingSystem.Web.Models;

public class DashboardViewModel
{
    public int UsersCount { get; set; }
    public int HotelsCount { get; set; }
    public int RoomsCount { get; set; }
    public int BookingsCount { get; set; }
    public decimal TotalRevenue { get; set; }

    public List<HotelDto> Hotels { get; set; } = new();
    public List<BookingDto> RecentBookings { get; set; } = new();

    // بيانات عرض إضافية (للتصميم فقط - لا تغيّر أي منطق عمل) — أرخص سعر ليلة متاح لكل فندق
    public Dictionary<int, decimal> HotelStartingPrice { get; set; } = new();
    public Dictionary<int, int> HotelAvailableRoomsCount { get; set; } = new();
}

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
