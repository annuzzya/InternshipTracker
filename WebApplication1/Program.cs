using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Data.Identity;
using WebApplication1.Extensions;
using WebApplication1.Infrastructure.Services;
using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<ApplicationIdentityContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(ApplicationIdentityContext))));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationIdentityContext>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IDataPortServiceFactory<Vacancy>, VacancyDataPortServiceFactory>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.InitializeRolesAsync();
    await app.InitializeDefaultUsersAsync(
        app.Configuration.GetSection("IdentityDefaults:SuperUser"),
        app.Configuration.GetSection("IdentityDefaults:DefaultUsers"));
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Companies}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

app.Run();
