namespace HotelBookingSystem.Application.DTOs.Hotel;

public class HotelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public double Rating { get; set; }
    public int RoomsCount { get; set; }
}
