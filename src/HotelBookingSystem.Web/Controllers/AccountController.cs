using System.Security.Claims;
using HotelBookingSystem.Application.DTOs.Auth;
using HotelBookingSystem.Application.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.Web.Controllers;

// [AllowAnonymous] هنا مهم جداً: كل الـ Controllers الأخرى محمية عالمياً (Program.cs)،
// لكن صفحة تسجيل الدخول نفسها يجب أن تبقى متاحة لأي زائر غير مسجّل دخول.
[AllowAnonymous]
public class AccountController : Controller
{
    private readonly IUserService _userService;

    public AccountController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: /Account/Login
    public IActionResult Login(string? returnUrl = null)
    {
        // إذا كان المستخدم مسجّل دخوله بالفعل، لا داعي لإعادة عرض صفحة الدخول
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToLocalOrHome(returnUrl);

        ViewData["Title"] = "تسجيل الدخول";
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginDto());
    }

    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto dto, string? returnUrl = null)
    {
        ViewData["Title"] = "تسجيل الدخول";
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(dto);

        var result = await _userService.AuthenticateAsync(dto);
        if (!result.Success || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "البريد الإلكتروني أو كلمة المرور غير صحيحة");
            return View(dto);
        }

        var auth = result.Data;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, auth.UserId.ToString()),
            new(ClaimTypes.Name, auth.FullName),
            new(ClaimTypes.Email, auth.Email),
            new(ClaimTypes.Role, auth.Role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = true });

        return RedirectToLocalOrHome(returnUrl);
    }

    // POST: /Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    // GET: /Account/AccessDenied
    public IActionResult AccessDenied()
    {
        ViewData["Title"] = "غير مصرح بالدخول";
        return View();
    }

    private IActionResult RedirectToLocalOrHome(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }
}
