using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Infrastructure.Services;

public class VacancyImportService : IImportService<Vacancy>
{
    private readonly AppDbContext _context;

    public VacancyImportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
    {
        if (!stream.CanRead)
        {
            throw new ArgumentException("Stream is not readable", nameof(stream));
        }

        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.FirstOrDefault();
        if (worksheet is null)
        {
            return;
        }

        foreach (var row in worksheet.RowsUsed().Skip(1))
        {
            await AddOrUpdateVacancyAsync(row, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task AddOrUpdateVacancyAsync(IXLRow row, CancellationToken cancellationToken)
    {
        var title = GetTitle(row);
        if (string.IsNullOrWhiteSpace(title))
        {
            return;
        }

        var companyName = GetCompanyName(row);
        var authorFullName = GetAuthorName(row);

        var company = await GetCompanyAsync(companyName, cancellationToken);
        var author = await GetAuthorAsync(authorFullName, cancellationToken);

        var vacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Title == title && v.CompanyId == company.Id, cancellationToken);
        if (vacancy is null)
        {
            vacancy = new Vacancy { Id = Guid.NewGuid() };
            _context.Vacancies.Add(vacancy);
        }

        vacancy.Title = title;
        vacancy.CompanyId = company.Id;
        vacancy.AuthorId = author.Id;
        vacancy.StatusId = GetStatusId(row);
        vacancy.SalaryMin = GetNullableInt(row, 5);
        vacancy.SalaryMax = GetNullableInt(row, 6);
        vacancy.Currency = GetString(row, 7);
        vacancy.Link = GetString(row, 8);
    }

    private async Task<Company> GetCompanyAsync(string companyName, CancellationToken cancellationToken)
    {
        var normalized = string.IsNullOrWhiteSpace(companyName) ? "Unknown Company" : companyName.Trim();
        var company = await _context.Companies.FirstOrDefaultAsync(c => c.Name == normalized, cancellationToken);
        if (company is null)
        {
            company = new Company { Id = Guid.NewGuid(), Name = normalized };
            _context.Companies.Add(company);
        }

        return company;
    }

    private async Task<User> GetAuthorAsync(string fullName, CancellationToken cancellationToken)
    {
        var normalized = string.IsNullOrWhiteSpace(fullName) ? "Unknown User" : fullName.Trim();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.FullName == normalized, cancellationToken);
        if (user is null)
        {
            user = new User { Id = Guid.NewGuid(), FullName = normalized, Email = $"{Guid.NewGuid():N}@import.local" };
            _context.Users.Add(user);
        }

        return user;
    }

    private static string GetTitle(IXLRow row) => GetString(row, 1);
    private static string GetCompanyName(IXLRow row) => GetString(row, 2);
    private static string GetAuthorName(IXLRow row) => GetString(row, 3);
    private static int GetStatusId(IXLRow row) => int.TryParse(GetString(row, 4), out var value) ? value : 3;

    private static int? GetNullableInt(IXLRow row, int cellIndex)
    {
        var raw = GetString(row, cellIndex);
        return int.TryParse(raw, out var value) ? value : null;
    }

    private static string GetString(IXLRow row, int cellIndex) => row.Cell(cellIndex).GetValue<string>().Trim();
}
