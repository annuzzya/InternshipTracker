using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Infrastructure.Services;

namespace WebApplication1.Controllers;

public class VacanciesController : Controller
{
    private readonly AppDbContext _context;
    private readonly IDataPortServiceFactory<Vacancy> _vacancyDataPortServiceFactory;

    public VacanciesController(AppDbContext context, IDataPortServiceFactory<Vacancy> vacancyDataPortServiceFactory)
    {
        _context = context;
        _vacancyDataPortServiceFactory = vacancyDataPortServiceFactory;
    }

    public async Task<IActionResult> Index(string searchString)
    {
        ViewData["CurrentFilter"] = searchString;
        var vacancies = from v in _context.Vacancies select v;

        if (!String.IsNullOrEmpty(searchString))
        {
            vacancies = vacancies.Where(s => s.Title.Contains(searchString));
        }
        return View(await vacancies.ToListAsync());
    }

// 1. Відкриває сторінку створення
    public IActionResult Create()
    {
        ViewBag.CompanyId = new SelectList(_context.Companies, "Id", "Name");
        ViewBag.AuthorId = new SelectList(_context.Users, "Id", "FullName");
        // ДОДАЛИ: Список статусів для форми створення
        ViewBag.StatusId = new List<SelectListItem> {
            new SelectListItem { Value = "1", Text = "Відкрита (Активна)" },
            new SelectListItem { Value = "2", Text = "Закрита" },
            new SelectListItem { Value = "3", Text = "Чернетка" }
        };
        return View();
    }

    // 2. Зберігає нову вакансію в базу
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Vacancy vacancy)
    {
        ModelState.Remove("Company");
        ModelState.Remove("Author");
        ModelState.Remove("Status");

        if (ModelState.IsValid)
        {
            _context.Add(vacancy);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        // Якщо помилка - повертаємо всі списки назад
        ViewBag.CompanyId = new SelectList(_context.Companies, "Id", "Name", vacancy.CompanyId);
        ViewBag.AuthorId = new SelectList(_context.Users, "Id", "FullName", vacancy.AuthorId);
        ViewBag.StatusId = new List<SelectListItem> {
            new SelectListItem { Value = "1", Text = "Відкрита (Активна)" },
            new SelectListItem { Value = "2", Text = "Закрита" },
            new SelectListItem { Value = "3", Text = "Чернетка" }
        };
        return View(vacancy);
    }
    
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id is null) return NotFound();
        var vacancy = await _context.Vacancies.FindAsync(id);
        if (vacancy is null) return NotFound();
        return View(vacancy);
    }

    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id is null) return NotFound();
        var vacancy = await _context.Vacancies.FindAsync(id);
        if (vacancy is null) return NotFound();

        ViewBag.CompanyId = new SelectList(_context.Companies, "Id", "Name", vacancy.CompanyId);
        ViewBag.AuthorId = new SelectList(_context.Users, "Id", "FullName", vacancy.AuthorId);
        ViewBag.StatusId = new List<SelectListItem> {
            new SelectListItem { Value = "1", Text = "Відкрита (Активна)" },
            new SelectListItem { Value = "2", Text = "Закрита" },
            new SelectListItem { Value = "3", Text = "Чернетка" }
        };
        return View(vacancy);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Vacancy vacancy)
    {
        if (id != vacancy.Id) return NotFound();

        // 1. Знаходимо в базі
        var existingVacancy = await _context.Vacancies.FindAsync(id);
        if (existingVacancy is null) return NotFound();

        ModelState.Remove("Company");
        ModelState.Remove("Author");
        ModelState.Remove("Status");

        if (ModelState.IsValid)
        {
            // 2. Оновлюємо ТІЛЬКИ безпечні поля
            existingVacancy.Title = vacancy.Title;
            existingVacancy.SalaryMin = vacancy.SalaryMin;
            existingVacancy.SalaryMax = vacancy.SalaryMax;
            existingVacancy.Currency = vacancy.Currency;
            existingVacancy.Link = vacancy.Link;
            existingVacancy.CompanyId = vacancy.CompanyId;
            existingVacancy.AuthorId = vacancy.AuthorId;
            existingVacancy.StatusId = vacancy.StatusId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        ViewBag.CompanyId = new SelectList(_context.Companies, "Id", "Name", vacancy.CompanyId);
        ViewBag.AuthorId = new SelectList(_context.Users, "Id", "FullName", vacancy.AuthorId);
        ViewBag.StatusId = new List<SelectListItem> {
            new SelectListItem { Value = "1", Text = "Відкрита (Активна)" },
            new SelectListItem { Value = "2", Text = "Закрита" },
            new SelectListItem { Value = "3", Text = "Чернетка" }
        };
        return View(vacancy);
    }


    [HttpGet]
    public IActionResult Import()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Import(IFormFile vacanciesFile, CancellationToken cancellationToken)
    {
        if (vacanciesFile is null || vacanciesFile.Length == 0)
        {
            ModelState.AddModelError(string.Empty, "Оберіть Excel-файл для імпорту.");
            return View();
        }

        var importService = _vacancyDataPortServiceFactory.GetImportService(vacanciesFile.ContentType);
        await using var stream = vacanciesFile.OpenReadStream();
        await importService.ImportFromStreamAsync(stream, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Export([FromQuery] string contentType = VacancyDataPortServiceFactory.ExcelContentType, CancellationToken cancellationToken = default)
    {
        var exportService = _vacancyDataPortServiceFactory.GetExportService(contentType);
        var memoryStream = new MemoryStream();

        await exportService.WriteToAsync(memoryStream, cancellationToken);
        await memoryStream.FlushAsync(cancellationToken);
        memoryStream.Position = 0;

        return new FileStreamResult(memoryStream, contentType)
        {
            FileDownloadName = $"vacancies_{DateTime.UtcNow:yyyy-MM-dd}.xlsx"
        };
    }
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id is null) return NotFound();
        var vacancy = await _context.Vacancies.FindAsync(id);
        if (vacancy is null) return NotFound();
        return View(vacancy);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var vacancy = await _context.Vacancies.FindAsync(id);
        if (vacancy is not null)
        {
            _context.Vacancies.Remove(vacancy);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}