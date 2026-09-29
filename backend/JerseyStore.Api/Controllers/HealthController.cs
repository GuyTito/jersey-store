using Microsoft.AspNetCore.Mvc;

namespace JerseyStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<HealthResponse>(StatusCodes.Status200OK)]
    public ActionResult<HealthResponse> Get()
    {
        return Ok(new HealthResponse(
            "ok",
            "Jersey Store API is running.",
            DateTimeOffset.UtcNow));
    }
}

public sealed record HealthResponse(
    string Status,
    string Message,
    DateTimeOffset Timestamp);
