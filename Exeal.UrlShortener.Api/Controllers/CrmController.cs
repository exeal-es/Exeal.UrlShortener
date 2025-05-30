using Exeal.NotionCrm.Infra;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exeal.UrlShortener.Api.Controllers;

//[Authorize]
[ApiController]
[Route("api/crm")]
public class CrmController(NotionCrmService notionCrmService) : ControllerBase
{
    [HttpGet("contacts")]
    public async Task<ActionResult> GetAllContacts()
    {
        var allContacts = await notionCrmService.GetAllContacts();
        return Ok(allContacts);
    }
    
    [HttpGet("contacts-by-email")]
    public async Task<ActionResult> GetContactInfoByWorkEmail(string email)
    {
        var results = (await notionCrmService.GetContactInfoByWorkEmail(email)).ToList();

        return results.Count switch
        {
            0 => NotFound($"Contact {email} no encontrado"),
            > 1 => Conflict($"Multiples contactos para {email} encontrados ({results.Count} resultados)"),
            _ => Ok(results.First())
        };
    }
}
