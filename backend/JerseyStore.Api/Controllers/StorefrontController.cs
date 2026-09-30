using JerseyStore.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace JerseyStore.Api.Controllers;

[ApiController]
[Route("api/storefront")]
public sealed class StorefrontController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public StorefrontController(IConfiguration configuration) => _configuration = configuration;

    [HttpGet]
    [ProducesResponseType<StorefrontResponse>(StatusCodes.Status200OK)]
    public ActionResult<StorefrontResponse> Get()
    {
        var number = _configuration["Store:WhatsAppNumber"]
            ?? throw new InvalidOperationException("Store:WhatsAppNumber must be configured.");

        return Ok(new StorefrontResponse(number, _configuration["Store:CurrencyCode"] ?? "GHS"));
    }
}