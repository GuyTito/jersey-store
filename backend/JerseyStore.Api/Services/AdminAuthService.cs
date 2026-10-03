using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JerseyStore.Api.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace JerseyStore.Api.Services;

public sealed class AdminAuthService(
    UserManager<IdentityUser> userManager,
    IConfiguration configuration,
    TimeProvider timeProvider) : IAdminAuthService
{
    private const string AdminRole = "Admin";
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromMinutes(30);

    public async Task<AdminLoginResponse?> LoginAsync(
        AdminLoginRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || await userManager.IsLockedOutAsync(user))
        {
            return null;
        }

        if (!await userManager.CheckPasswordAsync(user, request.Password))
        {
            if (user.LockoutEnabled)
            {
                EnsureSucceeded(await userManager.AccessFailedAsync(user), "recording failed administrator sign-in");
            }

            return null;
        }

        EnsureSucceeded(
            await userManager.ResetAccessFailedCountAsync(user),
            "resetting failed administrator sign-ins");
        if (!await userManager.IsInRoleAsync(user, AdminRole))
        {
            return null;
        }

        var now = timeProvider.GetUtcNow();
        var expiresAt = now.Add(TokenLifetime);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? request.Email),
            new Claim("name", user.Email ?? request.Email),
            new Claim("role", AdminRole),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
        };

        var issuer = configuration["Authentication:Jwt:Issuer"] ?? "JerseyStore.Api";
        var audience = configuration["Authentication:Jwt:Audience"] ?? "JerseyStore.Admin";
        var key = configuration["Authentication:Jwt:SigningKey"]
            ?? throw new InvalidOperationException("Authentication:Jwt:SigningKey must be configured.");
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: signingCredentials);

        return new AdminLoginResponse(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
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
