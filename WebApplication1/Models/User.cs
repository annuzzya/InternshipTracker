using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class User
{
    public Guid Id { get; set; }
    
    [Required]
    public string Email { get; set; }
    
    [Required]
    public string FullName { get; set; }
    
    [Required]
    public string Role { get; set; }
    
    public Guid? CompanyId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}