using System.ComponentModel.DataAnnotations;
using HotelBookingSystem.Domain.Enums;

namespace HotelBookingSystem.Application.DTOs.Booking;

public class UpdateBookingStatusDto
{
    [Required(ErrorMessage = "حالة الحجز مطلوبة")]
    public BookingStatus Status { get; set; }
}
