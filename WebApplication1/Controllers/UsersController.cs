using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication1.Controllers;

public class UsersController : Controller
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Users.ToListAsync());
    }

// 1. Відкриває сторінку і передає список компаній
    public IActionResult Create()
    {
        ViewBag.CompanyId = new SelectList(_context.Companies, "Id", "Name");
        return View();
    }

    // 2. Зберігає користувача
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(User user)
    {
        if (ModelState.IsValid)
        {
            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        // Якщо помилка (наприклад, пусте поле), список треба завантажити знову
        ViewBag.CompanyId = new SelectList(_context.Companies, "Id", "Name", user.CompanyId);
        return View(user);
    }
    // --- ДЕТАЛІ ---
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null) return NotFound();
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();
        return View(user);
    }

    // --- РЕДАГУВАННЯ (Відкрити сторінку) ---
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null) return NotFound();
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        // Передаємо список компаній (як при створенні)
        ViewBag.CompanyId = new SelectList(_context.Companies, "Id", "Name", user.CompanyId);
        return View(user);
    }

    // --- РЕДАГУВАННЯ (Зберегти зміни) ---
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, User user)
    {
        if (id != user.Id) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        ViewBag.CompanyId = new SelectList(_context.Companies, "Id", "Name", user.CompanyId);
        return View(user);
    }

    // --- ВИДАЛЕННЯ (Відкрити сторінку підтвердження) ---
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null) return NotFound();
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();
        return View(user);
    }

    // --- ВИДАЛЕННЯ (Підтвердити і видалити) ---
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}