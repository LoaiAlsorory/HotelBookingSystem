using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Application.DTOs.User;

public class CreateUserDto
{
    [Required(ErrorMessage = "الاسم الكامل مطلوب")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "الاسم يجب أن يكون بين 3 و 100 حرف")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
    [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "كلمة المرور مطلوبة")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "كلمة المرور يجب ألا تقل عن 6 أحرف")]
    public string Password { get; set; } = string.Empty;

    [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
    public string? PhoneNumber { get; set; }
}
