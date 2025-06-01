using Exeal.NotionCrm.Infra;
using Exeal.NotionCrm.Infra.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Exeal.UrlShortener.Api.Controllers;

//[Authorize]
[ApiController]
[Route("api/crm")]
public class CrmController(NotionCrmService notionCrmService) : ControllerBase
{
    [HttpGet("contacts/{id}")]
    public async Task<ActionResult> GetContactById(Guid id)
    {
        var result = await notionCrmService.GetContactById(id);
        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("contacts")]
    public async Task<ActionResult> FindContacts([FromQuery] string? email = null)
    {
        var foundContacts = await notionCrmService.FindContacts(email);
        return Ok(foundContacts);
    }
    
    [HttpPost("contacts")]
    public async Task<ActionResult> CreateContact([FromBody] CreateNotionContactDto request)
    {
        var result = await notionCrmService.CreateContact(request);
        if (result.IsFailure)
        {
            return Problem(result.Error);
        }

        var pageId = result.Value;
        return Created($"api/crm/contacts/{pageId}", new { Id = pageId });
    }
}
