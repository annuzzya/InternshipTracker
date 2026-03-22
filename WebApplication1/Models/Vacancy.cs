using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Vacancy
{
    public Guid Id { get; set; }
    
    public Guid CompanyId { get; set; }
    
    public Guid AuthorId { get; set; }
    
    public int StatusId { get; set; }
    
    [Required]
    public string Title { get; set; }
    
    public int? SalaryMin { get; set; }
    
    public int? SalaryMax { get; set; }
    
    public string? Currency { get; set; }
    
    public string? Link { get; set; }
}