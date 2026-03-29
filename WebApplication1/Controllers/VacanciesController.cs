using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication1.Controllers;

public class VacanciesController : Controller
{
    private readonly AppDbContext _context;

    public VacanciesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Vacancies.ToListAsync());
    }

    // 1. Відкриває сторінку і передає списки компаній та юзерів
    public IActionResult Create()
    {
        ViewBag.CompanyId = new SelectList(_context.Companies, "Id", "Name");
        ViewBag.AuthorId = new SelectList(_context.Users, "Id", "FullName");
        return View();
    }

    // 2. Зберігає вакансію
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Vacancy vacancy)
    {
        if (ModelState.IsValid)
        {
            _context.Add(vacancy);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        // Якщо помилка (наприклад, пусте поле), списки треба завантажити знову
        ViewBag.CompanyId = new SelectList(_context.Companies, "Id", "Name", vacancy.CompanyId);
        ViewBag.AuthorId = new SelectList(_context.Users, "Id", "FullName", vacancy.AuthorId);
        return View(vacancy);
    }
    // --- ДЕТАЛІ ---
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null) return NotFound();
        var vacancy = await _context.Vacancies.FindAsync(id);
        if (vacancy == null) return NotFound();
        return View(vacancy);
    }

    // --- РЕДАГУВАННЯ (Відкрити сторінку) ---
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null) return NotFound();
        var vacancy = await _context.Vacancies.FindAsync(id);
        if (vacancy == null) return NotFound();

        // Передаємо списки для випадаючих меню (як при створенні)
        ViewBag.CompanyId = new SelectList(_context.Companies, "Id", "Name", vacancy.CompanyId);
        ViewBag.AuthorId = new SelectList(_context.Users, "Id", "FullName", vacancy.AuthorId);
        ViewBag.StatusId = new List<SelectListItem> {
            new SelectListItem { Value = "1", Text = "Відкрита (Активна)" },
            new SelectListItem { Value = "2", Text = "Закрита" },
            new SelectListItem { Value = "3", Text = "Чернетка" }
        };
        return View(vacancy);
    }

    // --- РЕДАГУВАННЯ (Зберегти зміни) ---
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Vacancy vacancy)
    {
        if (id != vacancy.Id) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(vacancy);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        // Якщо помилка - повертаємо списки
        ViewBag.CompanyId = new SelectList(_context.Companies, "Id", "Name", vacancy.CompanyId);
        ViewBag.AuthorId = new SelectList(_context.Users, "Id", "FullName", vacancy.AuthorId);
        ViewBag.StatusId = new List<SelectListItem> {
            new SelectListItem { Value = "1", Text = "Відкрита (Активна)" },
            new SelectListItem { Value = "2", Text = "Закрита" },
            new SelectListItem { Value = "3", Text = "Чернетка" }
        };
        return View(vacancy);
    }

    // --- ВИДАЛЕННЯ (Відкрити сторінку підтвердження) ---
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null) return NotFound();
        var vacancy = await _context.Vacancies.FindAsync(id);
        if (vacancy == null) return NotFound();
        return View(vacancy);
    }

    // --- ВИДАЛЕННЯ (Підтвердити і видалити) ---
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var vacancy = await _context.Vacancies.FindAsync(id);
        if (vacancy != null)
        {
            _context.Vacancies.Remove(vacancy);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}