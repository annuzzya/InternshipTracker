using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Infrastructure.Services;

public class VacancyDataPortServiceFactory : IDataPortServiceFactory<Vacancy>
{
    public const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    private readonly AppDbContext _context;

    public VacancyDataPortServiceFactory(AppDbContext context)
    {
        _context = context;
    }

    public IImportService<Vacancy> GetImportService(string contentType)
    {
        if (contentType == ExcelContentType)
        {
            return new VacancyImportService(_context);
        }

        throw new NotImplementedException($"No import service implemented for vacancies with content type {contentType}");
    }

    public IExportService<Vacancy> GetExportService(string contentType)
    {
        if (contentType == ExcelContentType)
        {
            return new VacancyExportService(_context);
        }

        throw new NotImplementedException($"No export service implemented for vacancies with content type {contentType}");
    }
}
