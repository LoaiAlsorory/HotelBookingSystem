using HotelBookingSystem.Application.DTOs.User;
using HotelBookingSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service) => _service = service;

    /// <summary>عرض جميع المستخدمين (Index)</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll() =>
        Ok(await _service.GetAllAsync());

    /// <summary>عرض تفاصيل مستخدم (Details)</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var user = await _service.GetByIdAsync(id);
        if (user is null) return NotFound(new { message = $"المستخدم رقم {id} غير موجود" });
        return Ok(user);
    }

    /// <summary>إنشاء مستخدم جديد (Create)</summary>
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>تعديل مستخدم (Edit)</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updated = await _service.UpdateAsync(id, dto);
        if (!updated) return NotFound(new { message = $"المستخدم رقم {id} غير موجود" });
        return NoContent();
    }

    /// <summary>حذف مستخدم (Delete)</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(new { message = $"المستخدم رقم {id} غير موجود" });
        return NoContent();
    }
}
