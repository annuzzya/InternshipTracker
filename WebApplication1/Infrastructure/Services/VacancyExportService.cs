using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Infrastructure.Services;

public class VacancyExportService : IExportService<Vacancy>
{
    private readonly AppDbContext _context;

    private static readonly IReadOnlyList<string> HeaderNames =
    [
        "Назва", "Компанія", "Автор", "Статус", "Мін. зарплата", "Макс. зарплата", "Валюта", "Посилання"
    ];

    public VacancyExportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
    {
        if (!stream.CanWrite)
        {
            throw new ArgumentException("Input stream is not writable", nameof(stream));
        }

        var vacancies = await _context.Vacancies
            .Include(v => v.Company)
            .Include(v => v.Author)
            .ToListAsync(cancellationToken);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Vacancies");

        WriteHeader(worksheet);

        var rowIndex = 2;
        foreach (var vacancy in vacancies)
        {
            WriteVacancy(worksheet, vacancy, rowIndex++);
        }

        workbook.SaveAs(stream);
    }

    private static void WriteHeader(IXLWorksheet worksheet)
    {
        for (int i = 0; i < HeaderNames.Count; i++)
        {
            worksheet.Cell(1, i + 1).Value = HeaderNames[i];
        }

        worksheet.Row(1).Style.Font.Bold = true;
    }

    private static void WriteVacancy(IXLWorksheet worksheet, Vacancy vacancy, int rowIndex)
    {
        worksheet.Cell(rowIndex, 1).Value = vacancy.Title;
        worksheet.Cell(rowIndex, 2).Value = vacancy.Company?.Name ?? string.Empty;
        worksheet.Cell(rowIndex, 3).Value = vacancy.Author?.FullName ?? string.Empty;
        worksheet.Cell(rowIndex, 4).Value = vacancy.StatusId;
        worksheet.Cell(rowIndex, 5).Value = vacancy.SalaryMin;
        worksheet.Cell(rowIndex, 6).Value = vacancy.SalaryMax;
        worksheet.Cell(rowIndex, 7).Value = vacancy.Currency ?? string.Empty;
        worksheet.Cell(rowIndex, 8).Value = vacancy.Link ?? string.Empty;
    }
}
