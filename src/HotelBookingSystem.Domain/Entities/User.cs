using HotelBookingSystem.Domain.Enums;

namespace HotelBookingSystem.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public UserRole Role { get; set; } = UserRole.RegisteredUser;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
