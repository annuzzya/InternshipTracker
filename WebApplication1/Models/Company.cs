using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Company
{
    public Guid Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public string? Website { get; set; }
    
    public string? Location { get; set; }
}