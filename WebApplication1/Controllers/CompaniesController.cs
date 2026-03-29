using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

public class CompaniesController : Controller
{
    private readonly AppDbContext _context;

    public CompaniesController(AppDbContext context)
    {
        _context = context;
    }

    // Показує список всіх компаній
    public async Task<IActionResult> Index()
    {
        return View(await _context.Companies.ToListAsync());
    }
    
    // Відкриває сторінку деталей компанії та її вакансії
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null) return NotFound();

        // 1. Шукаємо саму компанію
        var company = await _context.Companies.FirstOrDefaultAsync(m => m.Id == id);
        if (company == null) return NotFound();

        // 2. Шукаємо всі вакансії, які належать цій компанії (по CompanyId)
        // Передаємо їх на сторінку через спеціальну "сумку" ViewBag
        ViewBag.Vacancies = await _context.Vacancies
            .Where(v => v.CompanyId == id)
            .ToListAsync();

        return View(company); // Віддаємо компанію на сторінку
    }

    // Відкриває сторінку створення
    public IActionResult Create()
    {
        return View();
    }

    // Зберігає нову компанію в базу даних
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Company company)
    {
        if (ModelState.IsValid)
        {
            _context.Add(company);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); // Повертає до списку
        }
        return View(company);
    }
    // 1. Відкриває сторінку з питанням "Ви впевнені?"
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null) return NotFound();

        var company = await _context.Companies.FirstOrDefaultAsync(m => m.Id == id);
        if (company == null) return NotFound();

        return View(company);
    }

    // 2. Кнопка підтвердження, яка РЕАЛЬНО видаляє з бази
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company != null)
        {
            _context.Companies.Remove(company);
            await _context.SaveChangesAsync(); // Зберігаємо зміни в базі
        }
        return RedirectToAction(nameof(Index));
    }
    // 1. Відкриває сторінку редагування і завантажує туди старі дані
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null) return NotFound();

        var company = await _context.Companies.FindAsync(id);
        if (company == null) return NotFound();
        
        return View(company);
    }

    // 2. Зберігає нові дані, які ти ввела
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Company company)
    {
        // Перевіряємо, чи не підмінили ID
        if (id != company.Id) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(company);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); // Повертаємось до списку
        }
        return View(company);
    }
}