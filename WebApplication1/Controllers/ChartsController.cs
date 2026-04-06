using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace WebApplication1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChartsController : ControllerBase
{
    private readonly AppDbContext _context;

    private record VacanciesByCompanyItem(string CompanyName, int Count);
    private record UsersByRoleItem(string Role, int Count);

    public ChartsController(AppDbContext context)
    {
        _context = context;
    }

    // 1. Графік: Вакансії по компаніях (Використовуємо JOIN)
    [HttpGet("vacanciesByCompany")]
    public async Task<JsonResult> GetVacanciesByCompany(CancellationToken cancellationToken)
    {
        // З'єднуємо таблиці явно, щоб не залежати від навігаційних властивостей
        var responseItems = await (from v in _context.Vacancies
                                   join c in _context.Companies on v.CompanyId equals c.Id
                                   group v by c.Name into g
                                   select new VacanciesByCompanyItem(g.Key, g.Count()))
                                   .ToListAsync(cancellationToken);

        return new JsonResult(responseItems);
    }

    // 2. Графік: Користувачі за ролями (Групуємо в пам'яті)
    [HttpGet("usersByRole")]
    public async Task<JsonResult> GetUsersByRole(CancellationToken cancellationToken)
    {
        // Витягуємо тільки колонку Role з бази даних, щоб не навантажувати мережу
        var roles = await _context.Users
                                  .Select(u => u.Role)
                                  .ToListAsync(cancellationToken);

        // Робимо групування вже на стороні C#, щоб уникнути помилок перекладу в SQL
        var responseItems = roles
            .GroupBy(role => string.IsNullOrEmpty(role) ? "Не вказано" : role)
            .Select(g => new UsersByRoleItem(g.Key, g.Count()))
            .ToList();

        return new JsonResult(responseItems);
    }
}