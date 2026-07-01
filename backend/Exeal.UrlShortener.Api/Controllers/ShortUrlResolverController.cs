using Exeal.UrlShortener.Ports.Input;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Exeal.UrlShortener.Api.Controllers;

[ApiController]
public class ShortUrlResolverController(IShortUrlResolver shortUrlResolver) : ControllerBase
{
    [HttpGet("{slug}")]
    [EnableRateLimiting("ResolveLimiter")]
    public async Task<IActionResult> Resolve(string slug)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();

        var result = await shortUrlResolver.ResolveAsync(slug, ipAddress, userAgent);

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Redirect(result.Value);
    }
}
