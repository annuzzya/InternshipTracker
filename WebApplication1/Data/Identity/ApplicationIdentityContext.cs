using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Data.Identity;

public class ApplicationIdentityContext(DbContextOptions<ApplicationIdentityContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
}
