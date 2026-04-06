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

    public async Task<IActionResult> Index(string searchString)
    {
        ViewData["CurrentFilter"] = searchString;
        var users = from u in _context.Users select u;

        if (!String.IsNullOrEmpty(searchString))
        {
            users = users.Where(s => s.FullName.Contains(searchString));
        }
        return View(await users.ToListAsync());
    }

    public IActionResult Create()
    {
        ViewBag.CompanyId = new SelectList(_context.Companies, "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(User user)
    {
        ModelState.Remove("Company"); 

        if (ModelState.IsValid)
        {
            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        ViewBag.CompanyId = new SelectList(_context.Companies, "Id", "Name", user.CompanyId);
        return View(user);
    }
    
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id is null) return NotFound();
        var user = await _context.Users.FindAsync(id);
        if (user is null) return NotFound();
        return View(user);
    }

    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id is null) return NotFound();
        var user = await _context.Users.FindAsync(id);
        if (user is null) return NotFound();

        ViewBag.CompanyId = new SelectList(_context.Companies, "Id", "Name", user.CompanyId);
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, User user)
    {
        if (id != user.Id) return NotFound();

        // 1. Знаходимо в базі
        var existingUser = await _context.Users.FindAsync(id);
        if (existingUser is null) return NotFound();

        ModelState.Remove("Company");

        if (ModelState.IsValid)
        {
            // 2. Оновлюємо ТІЛЬКИ безпечні поля
            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.Role = user.Role;
            existingUser.CompanyId = user.CompanyId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        ViewBag.CompanyId = new SelectList(_context.Companies, "Id", "Name", user.CompanyId);
        return View(user);
    }

    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id is null) return NotFound();
        var user = await _context.Users.FindAsync(id);
        if (user is null) return NotFound();
        return View(user);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is not null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}