namespace HotelBookingSystem.Application.DTOs.Booking;

public class BookingDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string HotelName { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int GuestsCount { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
