using System.Security.Claims;
using JerseyStore.Api.Contracts;
using JerseyStore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JerseyStore.Api.Controllers;

[ApiController]
[Route("api/admin/auth")]
public sealed class AdminAuthController(IAdminAuthService adminAuthService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<AdminLoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AdminLoginResponse>> Login(
        AdminLoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await adminAuthService.LoginAsync(request, cancellationToken);
        return result is null ? Unauthorized() : Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("me")]
    [ProducesResponseType<AdminIdentityResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<AdminIdentityResponse> GetCurrentAdministrator() =>
        Ok(new AdminIdentityResponse(User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? string.Empty));
}
