using System.ComponentModel.DataAnnotations;
using JerseyStore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JerseyStore.Api.Controllers;

public sealed record AdminSettingsResponse(string WhatsAppNumber);

public sealed record UpdateAdminSettingsRequest(
    [property: Required, StringLength(40)] string WhatsAppNumber);

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/settings")]
public sealed class AdminSettingsController(IStoreSettingsService settings) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<AdminSettingsResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminSettingsResponse>> Get(CancellationToken cancellationToken) =>
        Ok(new AdminSettingsResponse(await settings.GetWhatsAppNumberAsync(cancellationToken)));

    [HttpPut]
    [ProducesResponseType<AdminSettingsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AdminSettingsResponse>> Update(
        UpdateAdminSettingsRequest request,
        CancellationToken cancellationToken)
    {
        var saved = await settings.SetWhatsAppNumberAsync(request.WhatsAppNumber, cancellationToken);
        return saved is null
            ? BadRequest("Enter the number in international format, for example +233501234567.")
            : Ok(new AdminSettingsResponse(saved));
    }
}
