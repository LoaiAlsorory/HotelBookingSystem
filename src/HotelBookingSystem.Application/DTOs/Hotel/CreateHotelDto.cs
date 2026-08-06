using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Application.DTOs.Hotel;

public class CreateHotelDto
{
    [Required(ErrorMessage = "اسم الفندق مطلوب")]
    [StringLength(150, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "المدينة مطلوبة")]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "العنوان مطلوب")]
    [StringLength(250)]
    public string Address { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Url(ErrorMessage = "رابط الصورة غير صحيح")]
    public string? ImageUrl { get; set; }

    [Range(0, 5, ErrorMessage = "التقييم يجب أن يكون بين 0 و 5")]
    public double Rating { get; set; }
}
