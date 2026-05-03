using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChartsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ChartsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("vacanciesBySeniority")]
    public async Task<IActionResult> GetVacanciesBySeniority()
    {
        var titles = await _context.Vacancies
            .Where(v => v.Title != null)
            .Select(v => v.Title.ToLower())
            .ToListAsync();

        int intern = 0, junior = 0, middle = 0, senior = 0, lead = 0, architect = 0;

        foreach (var t in titles)
        {
            if (t.Contains("intern") || t.Contains("trainee")) intern++;
            else if (t.Contains("junior")) junior++;
            else if (t.Contains("senior")) senior++;
            else if (t.Contains("lead") || t.Contains("manager") || t.Contains("head")) lead++;
            else if (t.Contains("architect") || t.Contains("c-level") || t.Contains("expert")) architect++;
            else middle++;
        }

        return Ok(new[]
        {
            new { title = "Intern/Trainee", count = intern },
            new { title = "Junior", count = junior },
            new { title = "Middle", count = middle },
            new { title = "Senior", count = senior },
            new { title = "Lead / Manager", count = lead },
            new { title = "Architect / Top", count = architect }
        });
    }

    [HttpGet("vacanciesBySalary")]
    public async Task<IActionResult> GetVacanciesBySalary()
    {
        var vacancies = await _context.Vacancies.ToListAsync();

        int beginner = 0, standard = 0, premium = 0, negotiable = 0;

        foreach (var v in vacancies)
        {
            if (v.SalaryMin is null && v.SalaryMax is null)
            {
                negotiable++;
            }
            else
            {
                var salary = v.SalaryMax ?? v.SalaryMin ?? 0;
                if (salary < 800) beginner++;
                else if (salary >= 800 && salary <= 2000) standard++;
                else premium++;
            }
        }

        return Ok(new[]
        {
            new { category = "Стартові (до $800)", count = beginner },
            new { category = "Мідл ($800 - $2000)", count = standard },
            new { category = "Топові (понад $2000)", count = premium },
            new { category = "Оплата договірна", count = negotiable }
        });
    }
}