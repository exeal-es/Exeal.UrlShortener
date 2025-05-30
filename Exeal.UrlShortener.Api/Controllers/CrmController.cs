using Exeal.NotionCrm.Infra;
using Microsoft.AspNetCore.Authorization;
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
}
