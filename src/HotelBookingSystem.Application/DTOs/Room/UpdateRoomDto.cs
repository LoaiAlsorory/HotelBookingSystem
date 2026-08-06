using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Application.DTOs.Room;

public class UpdateRoomDto
{
    [Required]
    [StringLength(20)]
    public string RoomNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string RoomType { get; set; } = string.Empty;

    [Required]
    [Range(0.01, 100000)]
    public decimal PricePerNight { get; set; }

    [Required]
    [Range(1, 20)]
    public int Capacity { get; set; }

    [Url]
    public string? ImageUrl { get; set; }

    public bool IsAvailable { get; set; }
}
