using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Vacancy
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Оберіть компанію")]
    [Display(Name = "Компанія")]
    public Guid CompanyId { get; set; }

    [Required(ErrorMessage = "Оберіть автора")]
    [Display(Name = "Автор")]
    public Guid AuthorId { get; set; }

    [Display(Name = "Статус")]
    public int StatusId { get; set; } = 1;

    [Required(ErrorMessage = "Назва вакансії обов'язкова")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Назва від 2 до 200 символів")]
    [Display(Name = "Назва вакансії")]
    public string Title { get; set; } = string.Empty;

    [Range(0, 1_000_000, ErrorMessage = "Значення від 0 до 1 000 000")]
    [Display(Name = "Мінімальна зарплата")]
    public int? SalaryMin { get; set; }

    [Range(0, 1_000_000, ErrorMessage = "Значення від 0 до 1 000 000")]
    [Display(Name = "Максимальна зарплата")]
    public int? SalaryMax { get; set; }

    [StringLength(10)]
    [Display(Name = "Валюта")]
    public string? Currency { get; set; }

    [Url(ErrorMessage = "Введіть коректне посилання (https://...)")]
    [Display(Name = "Посилання на вакансію")]
    public string? Link { get; set; }

    public Company? Company { get; set; }
    public User? Author { get; set; }
}
