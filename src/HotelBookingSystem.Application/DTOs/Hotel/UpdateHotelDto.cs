using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Application.DTOs.Hotel;

public class UpdateHotelDto
{
    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(250)]
    public string Address { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Url]
    public string? ImageUrl { get; set; }

    [Range(0, 5)]
    public double Rating { get; set; }
}
