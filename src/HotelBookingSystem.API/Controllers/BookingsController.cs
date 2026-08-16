using HotelBookingSystem.Application.Common;
using HotelBookingSystem.Application.DTOs.Booking;
using HotelBookingSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _service;

    public BookingsController(IBookingService service) => _service = service;

    /// <summary>عرض جميع الحجوزات، أو حجوزات مستخدم محدد عبر ?userId= (Index)</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetAll([FromQuery] int? userId)
    {
        var bookings = userId.HasValue
            ? await _service.GetByUserIdAsync(userId.Value)
            : await _service.GetAllAsync();

        return Ok(bookings);
    }

    /// <summary>عرض تفاصيل حجز (Details)</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingDto>> GetById(int id)
    {
        var booking = await _service.GetByIdAsync(id);
        if (booking is null) return NotFound(new { message = $"الحجز رقم {id} غير موجود" });
        return Ok(booking);
    }

    /// <summary>
    /// إنشاء حجز جديد (Create) — يطبّق التحقق من صحة المدخلات والسعة ومنع تعارض الحجز (FR4)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BookingDto>> Create([FromBody] CreateBookingDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _service.CreateAsync(dto);
        if (!result.Success)
        {
            return result.Type switch
            {
                ResultType.NotFound => NotFound(new { message = result.Error }),
                ResultType.Conflict => Conflict(new { message = result.Error }),
                ResultType.BadRequest => BadRequest(new { message = result.Error }),
                _ => BadRequest(new { message = result.Error })
            };
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>تعديل حالة الحجز — تأكيد/إلغاء/إكمال (Edit)</summary>
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateBookingStatusDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updated = await _service.UpdateStatusAsync(id, dto);
        if (!updated) return NotFound(new { message = $"الحجز رقم {id} غير موجود" });
        return NoContent();
    }

    /// <summary>حذف حجز (Delete)</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(new { message = $"الحجز رقم {id} غير موجود" });
        return NoContent();
    }
}

