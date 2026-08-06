using HotelBookingSystem.Application.DTOs.Hotel;
using HotelBookingSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _service;

    public HotelsController(IHotelService service) => _service = service;

    /// <summary>عرض جميع الفنادق (Index)</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HotelDto>>> GetAll() =>
        Ok(await _service.GetAllAsync());

    /// <summary>عرض تفاصيل فندق مع غرفه (Details)</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<HotelDto>> GetById(int id)
    {
        var hotel = await _service.GetByIdAsync(id);
        if (hotel is null) return NotFound(new { message = $"الفندق رقم {id} غير موجود" });
        return Ok(hotel);
    }

    /// <summary>إنشاء فندق جديد (Create)</summary>
    [HttpPost]
    public async Task<ActionResult<HotelDto>> Create([FromBody] CreateHotelDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>تعديل فندق (Edit)</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateHotelDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updated = await _service.UpdateAsync(id, dto);
        if (!updated) return NotFound(new { message = $"الفندق رقم {id} غير موجود" });
        return NoContent();
    }

    /// <summary>حذف فندق (Delete)</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound(new { message = $"الفندق رقم {id} غير موجود" });
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
