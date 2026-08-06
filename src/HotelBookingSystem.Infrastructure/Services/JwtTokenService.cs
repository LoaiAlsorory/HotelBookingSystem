using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HotelBookingSystem.Application.Interfaces;
using HotelBookingSystem.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HotelBookingSystem.Infrastructure.Services;

public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _config;

    public JwtTokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(User user)
    {
        var secretKey = _config["JwtSettings:Secret"] ?? "HotelBookingSystemSecretKey_FieldTraining_W5_2026_SecureKey_AlHikmaUniversity";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"] ?? "HotelBookingSystemAPI",
            audience: _config["JwtSettings:Audience"] ?? "HotelBookingSystemClient",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
