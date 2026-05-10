using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Company
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Назва компанії обов'язкова")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Назва від 2 до 200 символів")]
    [Display(Name = "Назва компанії")]
    public string Name { get; set; } = string.Empty;

    [Url(ErrorMessage = "Введіть коректний URL (https://...)")]
    [Display(Name = "Веб-сайт")]
    public string? Website { get; set; }

    [StringLength(100)]
    [Display(Name = "Локація")]
    public string? Location { get; set; }
}
