using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Application.DTOs.Room;

public class CreateRoomDto
{
    [Required(ErrorMessage = "معرّف الفندق مطلوب")]
    public int HotelId { get; set; }

    [Required(ErrorMessage = "رقم الغرفة مطلوب")]
    [StringLength(20)]
    public string RoomNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "نوع الغرفة مطلوب")]
    [StringLength(50)]
    public string RoomType { get; set; } = string.Empty;

    [Required(ErrorMessage = "السعر لكل ليلة مطلوب")]
    [Range(0.01, 100000, ErrorMessage = "السعر يجب أن يكون أكبر من صفر")]
    public decimal PricePerNight { get; set; }

    [Required(ErrorMessage = "السعة مطلوبة")]
    [Range(1, 20, ErrorMessage = "السعة يجب أن تكون بين 1 و 20")]
    public int Capacity { get; set; }

    [Url(ErrorMessage = "رابط الصورة غير صحيح")]
    public string? ImageUrl { get; set; }

    public bool IsAvailable { get; set; } = true;
}
