using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Threading.Tasks;

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

    public IActionResult Create()
    {
        return View();
    }

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
        return View(vacancy);
    }
}