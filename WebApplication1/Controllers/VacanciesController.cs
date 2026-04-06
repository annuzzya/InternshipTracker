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

    public async Task<IActionResult> Index(string searchString)
    {
        // Зберігаємо запит, щоб вставити його назад у рядок пошуку на сторінці
        ViewData["CurrentFilter"] = searchString;

        var vacancies = from v in _context.Vacancies select v;

        // Якщо рядок пошуку не пустий - шукаємо по назві (Title)
        if (!String.IsNullOrEmpty(searchString))
        {
            vacancies = vacancies.Where(s => s.Title.Contains(searchString));
        }

        return View(await vacancies.ToListAsync());
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
        // Ігноруємо навігаційні властивості при валідації
        ModelState.Remove("Company");
        ModelState.Remove("Author");
        ModelState.Remove("Status");

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

        // Ігноруємо навігаційні властивості при валідації
        ModelState.Remove("Company");
        ModelState.Remove("Author");
        ModelState.Remove("Status");

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(vacancy);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VacancyExists(vacancy.Id)) return NotFound();
                else throw;
            }
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

    // Допоміжний метод для перевірки існування
    private bool VacancyExists(Guid id)
    {
        return _context.Vacancies.Any(e => e.Id == id);
    }
}