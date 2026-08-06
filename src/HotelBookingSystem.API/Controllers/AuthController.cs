using HotelBookingSystem.Application.Common;
using HotelBookingSystem.Application.DTOs.Auth;
using HotelBookingSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService) => _userService = userService;

    /// <summary>تسجيل حساب جديد للمستخدم (Register)</summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _userService.RegisterAsync(dto);
        if (!result.Success)
        {
            if (result.Type == ResultType.Conflict)
                return Conflict(new { message = result.Error });

            return BadRequest(new { message = result.Error });
        }

        return Ok(result.Data);
    }

    /// <summary>تسجيل الدخول والحصول على رمز التوثيق (Login / JWT Token)</summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _userService.AuthenticateAsync(dto);
        if (!result.Success)
        {
            return Unauthorized(new { message = result.Error });
        }

        return Ok(result.Data);
    }
}
