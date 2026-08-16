using System.ComponentModel.DataAnnotations;
using HotelBookingSystem.Domain.Enums;

namespace HotelBookingSystem.Application.DTOs.Booking;

// يُستخدم من واجهة الويب (MVC) للتعديل الكامل على حجز موجود:
// الغرفة، تاريخ الدخول/الخروج، عدد الضيوف، وحالة الحجز.
// (الـ Web API يستمر باستخدام UpdateBookingStatusDto لتعديل الحالة فقط، حسب توثيق W5/W6)
public class UpdateBookingDto : IValidatableObject
{
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

    [Required(ErrorMessage = "حالة الحجز مطلوبة")]
    public BookingStatus Status { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CheckOutDate <= CheckInDate)
        {
            yield return new ValidationResult(
                "تاريخ الخروج يجب أن يكون بعد تاريخ الدخول",
                new[] { nameof(CheckOutDate) });
        }
    }
}
