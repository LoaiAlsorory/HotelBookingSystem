using HotelBookingSystem.Application.DTOs.Room;
using HotelBookingSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _service;

    public RoomsController(IRoomService service) => _service = service;

    /// <summary>عرض جميع الغرف، أو غرف فندق محدد عبر ?hotelId= (Index)</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetAll([FromQuery] int? hotelId)
    {
        var rooms = hotelId.HasValue
            ? await _service.GetByHotelIdAsync(hotelId.Value)
            : await _service.GetAllAsync();

        return Ok(rooms);
    }

    /// <summary>عرض تفاصيل غرفة (Details)</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RoomDto>> GetById(int id)
    {
        var room = await _service.GetByIdAsync(id);
        if (room is null) return NotFound(new { message = $"الغرفة رقم {id} غير موجودة" });
        return Ok(room);
    }

    /// <summary>إنشاء غرفة جديدة (Create)</summary>
    [HttpPost]
    public async Task<ActionResult<RoomDto>> Create([FromBody] CreateRoomDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>تعديل غرفة (Edit)</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updated = await _service.UpdateAsync(id, dto);
        if (!updated) return NotFound(new { message = $"الغرفة رقم {id} غير موجودة" });
        return NoContent();
    }

    /// <summary>حذف غرفة (Delete)</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound(new { message = $"الغرفة رقم {id} غير موجودة" });
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
