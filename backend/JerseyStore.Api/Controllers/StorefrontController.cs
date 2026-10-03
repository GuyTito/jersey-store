using JerseyStore.Api.Contracts;
using JerseyStore.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace JerseyStore.Api.Controllers;

[ApiController]
[Route("api/storefront")]
public sealed class StorefrontController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IStoreSettingsService _settings;

    public StorefrontController(IConfiguration configuration, IStoreSettingsService settings)
    {
        _configuration = configuration;
        _settings = settings;
    }

    [HttpGet]
    [ProducesResponseType<StorefrontResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<StorefrontResponse>> Get(CancellationToken cancellationToken)
    {
        var number = await _settings.GetWhatsAppNumberAsync(cancellationToken);

        return Ok(new StorefrontResponse(number, _configuration["Store:CurrencyCode"] ?? "GHS"));
    }
}
