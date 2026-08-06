using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Application.DTOs.Booking;

public class CreateBookingDto : IValidatableObject
{
    [Required(ErrorMessage = "معرّف المستخدم مطلوب")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "معرّف الغرفة مطلوب")]
    public int RoomId { get; set; }

    [Required(ErrorMessage = "تاريخ الدخول مطلوب")]
    [DataType(DataType.Date)]
    public DateTime CheckInDate { get; set; }

    [Required(ErrorMessage = "تاريخ الخروج مطلوب")]
    [DataType(DataType.Date)]
    public DateTime CheckOutDate { get; set; }

    [Required(ErrorMessage = "عدد الضيوف مطلوب")]
    [Range(1, 10, ErrorMessage = "عدد الضيوف يجب أن يكون بين 1 و 10")]
    public int GuestsCount { get; set; }

    // تحقق مخصص: تاريخ الخروج يجب أن يكون بعد تاريخ الدخول (FR3/FR4)
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CheckOutDate <= CheckInDate)
        {
            yield return new ValidationResult(
                "تاريخ الخروج يجب أن يكون بعد تاريخ الدخول",
                new[] { nameof(CheckOutDate) });
        }

        if (CheckInDate.Date < DateTime.UtcNow.Date)
        {
            yield return new ValidationResult(
                "تاريخ الدخول لا يمكن أن يكون في الماضي",
                new[] { nameof(CheckInDate) });
        }
    }
}
