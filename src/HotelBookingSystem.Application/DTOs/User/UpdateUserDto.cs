using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Application.DTOs.User;

public class UpdateUserDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string FullName { get; set; } = string.Empty;

    [Phone]
    public string? PhoneNumber { get; set; }
}
