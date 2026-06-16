using Exeal.UrlShortener.Api.Models;
using Exeal.UrlShortener.Ports.Input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exeal.UrlShortener.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/shorturl")]
public class ShortUrlManagerController(IShortUrlManager shortUrlManager) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateShortUrlRequest request)
    {
        var result = await shortUrlManager.CreateAsync(request.DestinationUrl, request.CustomSlug, request.Title);
        
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });
            
        return Ok(new { slug = result.Value });
    }

    [HttpGet("{slug}/stats")]
    public async Task<IActionResult> GetStats(string slug)
    {
        var result = await shortUrlManager.GetStatsAsync(slug);
        
        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });
            
        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 10)
    {
        if (skip < 0)
            return BadRequest(new { error = "Skip parameter cannot be negative." });
            
        if (take <= 0 || take > 100)
            return BadRequest(new { error = "Take parameter must be between 1 and 100." });

        var result = await shortUrlManager.ListAsync(skip, take);
        
        if (!result.IsSuccess)
            return StatusCode(500, new { error = result.Error });
            
        return Ok(result.Value);
    }
}
