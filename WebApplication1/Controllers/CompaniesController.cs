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

    public async Task<IActionResult> Index(string searchString)
    {
        ViewData["CurrentFilter"] = searchString;
        var companies = from c in _context.Companies select c;

        if (!String.IsNullOrEmpty(searchString))
        {
            companies = companies.Where(s => s.Name.Contains(searchString));
        }
        return View(await companies.ToListAsync());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Company company)
    {
        if (ModelState.IsValid)
        {
            _context.Add(company);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); 
        }
        return View(company);
    }

    public async Task<IActionResult> Details(Guid? id)
    {
        if (id is null) return NotFound();
        var company = await _context.Companies.FindAsync(id);
        if (company is null) return NotFound();
        return View(company);
    }

    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id is null) return NotFound();
        var company = await _context.Companies.FindAsync(id);
        if (company is null) return NotFound();
        return View(company);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Company company)
    {
        if (id != company.Id) return NotFound();

        var existingCompany = await _context.Companies.FindAsync(id);
        if (existingCompany is null) return NotFound();

        if (ModelState.IsValid)
        {
            existingCompany.Name = company.Name;
            existingCompany.Website = company.Website;
            existingCompany.Location = company.Location;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); 
        }
        return View(company);
    }

    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id is null) return NotFound();
        var company = await _context.Companies.FindAsync(id);
        if (company is null) return NotFound();
        return View(company);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company is not null)
        {
            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}