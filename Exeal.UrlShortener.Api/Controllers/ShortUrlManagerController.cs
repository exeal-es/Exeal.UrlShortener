using Exeal.UrlShortener.Api.Models;
using Exeal.UrlShortener.Ports.Input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exeal.UrlShortener.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/shorturl")]
public class ShortUrlManagerController : ControllerBase
{
    private readonly IShortUrlManager _shortUrlManager;

    public ShortUrlManagerController(IShortUrlManager shortUrlManager)
    {
        _shortUrlManager = shortUrlManager;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateShortUrlRequest request)
    {
        var result = await _shortUrlManager.CreateAsync(request.DestinationUrl, request.CustomSlug);
        
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });
            
        return Ok(new { slug = result.Value });
    }

    [HttpGet("{slug}/stats")]
    public async Task<IActionResult> GetStats(string slug)
    {
        var result = await _shortUrlManager.GetStatsAsync(slug);
        
        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });
            
        return Ok(result.Value);
    }
} 