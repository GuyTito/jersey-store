using Microsoft.AspNetCore.Identity;

namespace JerseyStore.Api.Services;

public sealed class AdminUserBootstrapper(
    UserManager<IdentityUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration,
    ILogger<AdminUserBootstrapper> logger) : IAdminUserBootstrapper
{
    private const string AdminRole = "Admin";

    public async Task EnsureAdminAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!await roleManager.RoleExistsAsync(AdminRole))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole(AdminRole));
            EnsureSucceeded(roleResult, "creating the Admin role");
        }

        var email = configuration["AdminBootstrap:Email"];
        var password = configuration["AdminBootstrap:Password"];
        if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "AdminBootstrap:Email and AdminBootstrap:Password must either both be configured or both be omitted.");
        }

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                LockoutEnabled = true,
            };
            var createResult = await userManager.CreateAsync(user, password);
            EnsureSucceeded(createResult, "creating the bootstrap administrator");
            logger.LogInformation("Created the configured bootstrap administrator.");
        }

        if (!await userManager.IsInRoleAsync(user, AdminRole))
        {
            var addRoleResult = await userManager.AddToRoleAsync(user, AdminRole);
            EnsureSucceeded(addRoleResult, "assigning the Admin role to the bootstrap administrator");
        }
    }

    private static void EnsureSucceeded(IdentityResult result, string operation)
    {
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(error => $"{error.Code}: {error.Description}"));
            throw new InvalidOperationException($"Identity failed while {operation}: {errors}");
        }
    }
}
