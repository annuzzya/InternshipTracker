using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class User
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Email обов'язковий")]
    [EmailAddress(ErrorMessage = "Введіть коректний email")]
    [StringLength(200)]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "ПІБ обов'язкове")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "ПІБ від 2 до 200 символів")]
    [Display(Name = "ПІБ")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Роль обов'язкова")]
    [StringLength(50)]
    [Display(Name = "Роль")]
    public string Role { get; set; } = "User";

    [Display(Name = "Компанія (ID)")]
    public Guid? CompanyId { get; set; }

    [Display(Name = "Дата реєстрації")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
