namespace HotelBookingSystem.Domain.Entities;

public class Room
{
    public int Id { get; set; }

    public int HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    public string RoomNumber { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int Capacity { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; } = true;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
