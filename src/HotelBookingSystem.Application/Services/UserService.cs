using System.Security.Cryptography;
using System.Text;
using HotelBookingSystem.Application.Common;
using HotelBookingSystem.Application.DTOs.Auth;
using HotelBookingSystem.Application.DTOs.User;
using HotelBookingSystem.Application.Interfaces;
using HotelBookingSystem.Domain.Entities;
using HotelBookingSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;
    private readonly ITokenService _tokenService;

    public UserService(IUserRepository repo, ITokenService tokenService)
    {
        _repo = repo;
        _tokenService = tokenService;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _repo.GetAllAsync();
        return users.Select(ToDto);
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _repo.GetByIdAsync(id);
        return user is null ? null : ToDto(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        var existing = await _repo.GetByEmailAsync(dto.Email);
        if (existing is not null)
            throw new InvalidOperationException("البريد الإلكتروني مستخدم بالفعل");

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password),
            PhoneNumber = dto.PhoneNumber,
            Role = UserRole.RegisteredUser
        };

        var created = await _repo.AddAsync(user);
        return ToDto(created);
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto)
    {
        var existing = await _repo.GetByEmailAsync(dto.Email);
        if (existing is not null)
            return Result<AuthResponseDto>.Conflict("البريد الإلكتروني مستخدم بالفعل");

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password),
            PhoneNumber = dto.PhoneNumber,
            Role = UserRole.RegisteredUser
        };

        var created = await _repo.AddAsync(user);
        var token = _tokenService.GenerateToken(created);

        return Result<AuthResponseDto>.Ok(new AuthResponseDto
        {
            Token = token,
            UserId = created.Id,
            FullName = created.FullName,
            Email = created.Email,
            Role = created.Role.ToString()
        });
    }

    public async Task<Result<AuthResponseDto>> AuthenticateAsync(LoginDto dto)
    {
        var user = await _repo.GetByEmailAsync(dto.Email);
        if (user is null || !VerifyPassword(dto.Password, user.PasswordHash))
            return Result<AuthResponseDto>.Unauthorized("البريد الإلكتروني أو كلمة المرور غير صحيحة");

        var token = _tokenService.GenerateToken(user);

        return Result<AuthResponseDto>.Ok(new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString()
        });
    }

    public async Task<bool> UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user is null) return false;

        user.FullName = dto.FullName;
        user.PhoneNumber = dto.PhoneNumber;

        await _repo.UpdateAsync(user);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user is null) return false;

        try
        {
            await _repo.DeleteAsync(user);
            return true;
        }
        catch (DbUpdateException)
        {
            // القيد Restrict على User->Booking يمنع الحذف إذا كان لدى المستخدم حجوزات
            throw new InvalidOperationException("لا يمكن حذف هذا المستخدم لوجود حجوزات مرتبطة به. يجب حذف حجوزاته أولًا.");
        }
    }

    // تشفير كلمة المرور باستخدام PBKDF2 المدمج مع Salt عشوائي (آمن ومطابق للممارسات القياسية)
    public static string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password: Encoding.UTF8.GetBytes(password),
            salt: salt,
            iterations: 100000,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: 32
        );

        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split(':');
        if (parts.Length != 2)
        {
            // يدعم التشفير القديم البسيط كمرحلة انتقال إذا لزم الأمر
            var legacyHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(legacyHash),
                Encoding.UTF8.GetBytes(storedHash)
            );
        }

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] expectedHash = Convert.FromBase64String(parts[1]);

        byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
            password: Encoding.UTF8.GetBytes(password),
            salt: salt,
            iterations: 100000,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: 32
        );

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }

    private static UserDto ToDto(User u) => new()
    {
        Id = u.Id,
        FullName = u.FullName,
        Email = u.Email,
        PhoneNumber = u.PhoneNumber,
        Role = u.Role.ToString(),
        CreatedAt = u.CreatedAt
    };
}

