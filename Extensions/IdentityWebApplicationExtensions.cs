using Microsoft.AspNetCore.Identity;
using WebApplication1.Data.Identity;

namespace WebApplication1.Extensions;

public static class IdentityWebApplicationExtensions
{
    private record UserInfo(string Username, string Password)
    {
        public UserInfo() : this(string.Empty, string.Empty)
        {
        }
    }

    private static async Task AddUserIfNotExistsAsync(
        UserManager<ApplicationUser> userManager,
        ILogger logger,
        string userName,
        string password,
        IReadOnlyCollection<string> roles)
    {
        var applicationUser = await userManager.FindByEmailAsync(userName);
        if (applicationUser is null)
        {
            applicationUser = new ApplicationUser
            {
                UserName = userName,
                Email = userName,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(applicationUser, password);
            if (!createResult.Succeeded)
            {
                logger.LogWarning("Cannot create user {username}: {errors}", userName, string.Join("; ", createResult.Errors.Select(e => e.Description)));
                return;
            }

            logger.LogInformation("{username} user added", userName);
        }

        var existingRoles = await userManager.GetRolesAsync(applicationUser);
        foreach (var role in roles.Where(role => !existingRoles.Contains(role)))
        {
            var addToRoleResult = await userManager.AddToRoleAsync(applicationUser, role);
            if (addToRoleResult.Succeeded)
            {
                logger.LogInformation("{username} has {rolename} assigned", userName, role);
            }
        }
    }

    public static async Task InitializeRolesAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var roleName in RoleNames.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    public static async Task InitializeDefaultUsersAsync(
        this WebApplication app,
        IConfiguration? superUserConfiguration,
        IConfiguration? defaultUsersConfiguration)
    {
        using var scope = app.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var superUserInfo = superUserConfiguration?.Get<UserInfo>();
        if (superUserInfo is not null)
        {
            await AddUserIfNotExistsAsync(userManager, app.Logger, superUserInfo.Username, superUserInfo.Password, RoleNames.All);
        }

        var defaultUsers = defaultUsersConfiguration?.Get<UserInfo[]>();
        if (defaultUsers is null)
        {
            return;
        }

        foreach (var defaultUserInfo in defaultUsers)
        {
            await AddUserIfNotExistsAsync(userManager, app.Logger, defaultUserInfo.Username, defaultUserInfo.Password, [RoleNames.User]);
        }
    }
}
