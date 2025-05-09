using Exeal.UrlShortener.Ports.Input;
using Microsoft.AspNetCore.Mvc;

namespace Exeal.UrlShortener.Api.Controllers;

[ApiController]
public class ShortUrlResolverController : ControllerBase
{
    private readonly IShortUrlResolver _shortUrlResolver;

    public ShortUrlResolverController(IShortUrlResolver shortUrlResolver)
    {
        _shortUrlResolver = shortUrlResolver;
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> Resolve(string slug)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
        
        var result = await _shortUrlResolver.ResolveAsync(slug, ipAddress, userAgent);
        
        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });
            
        return Redirect(result.Value);
    }
} 