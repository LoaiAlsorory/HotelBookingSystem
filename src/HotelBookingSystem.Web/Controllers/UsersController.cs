using HotelBookingSystem.Application.DTOs.User;
using HotelBookingSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.Web.Controllers;

public class UsersController : Controller
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: /Users
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "المستخدمون";
        var users = await _userService.GetAllAsync();
        return View(users);
    }

    // GET: /Users/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user is null) return NotFound();

        ViewData["Title"] = "تفاصيل المستخدم";
        return View(user);
    }

    // GET: /Users/Create
    public IActionResult Create()
    {
        ViewData["Title"] = "إضافة مستخدم";
        return View();
    }

    // POST: /Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserDto dto)
    {
        ViewData["Title"] = "إضافة مستخدم";
        if (!ModelState.IsValid) return View(dto);

        try
        {
            await _userService.CreateAsync(dto);
            TempData["Success"] = "تمت إضافة المستخدم بنجاح";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(dto);
        }
    }

    // GET: /Users/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user is null) return NotFound();

        var dto = new UpdateUserDto
        {
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber
        };

        ViewData["Title"] = "تعديل مستخدم";
        ViewBag.UserId = id;
        ViewBag.Email = user.Email;
        return View(dto);
    }

    // POST: /Users/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateUserDto dto)
    {
        ViewData["Title"] = "تعديل مستخدم";
        if (!ModelState.IsValid)
        {
            ViewBag.UserId = id;
            var existing = await _userService.GetByIdAsync(id);
            ViewBag.Email = existing?.Email;
            return View(dto);
        }

        var updated = await _userService.UpdateAsync(id, dto);
        if (!updated) return NotFound();

        TempData["Success"] = "تم تعديل بيانات المستخدم بنجاح";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Users/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user is null) return NotFound();

        ViewData["Title"] = "حذف مستخدم";
        return View(user);
    }

    // POST: /Users/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var deleted = await _userService.DeleteAsync(id);
            TempData[deleted ? "Success" : "Error"] = deleted
                ? "تم حذف المستخدم بنجاح"
                : "تعذّر حذف المستخدم، قد يكون غير موجود";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }
}
