using Exeal.UrlShortener.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notion.Client;

namespace Exeal.UrlShortener.Api.Controllers;

//[Authorize]
[ApiController]
[Route("api/crm")]
public class CrmController(IConfiguration configuration) : ControllerBase
{
    [HttpGet("contacts")]
    public async Task<ActionResult> GetAllContacts()
    {
        var notionClient = NotionClientFactory.Create(new ClientOptions
        {
            AuthToken = configuration["NotionToken"]
        });

        var contactsDatabaseId = "ce55f603a4c54881bee92f4fee5eef42";
        var results = await notionClient.Databases.QueryAsync(contactsDatabaseId, new DatabasesQueryParameters());

        var jsonResults = results.Results.Select(page => Notion2Markdown.ExportPageToJson(page)).ToList();
        
        return Ok(jsonResults);
    }
    
    [HttpGet("contacts-by-email")]
    public async Task<ActionResult> GetContactInfoByWorkEmail(string email)
    {
        var notionClient = NotionClientFactory.Create(new ClientOptions
        {
            AuthToken = configuration["NotionToken"]
        });

        var contactsDatabaseId = "ce55f603a4c54881bee92f4fee5eef42";
        var results = await notionClient.Databases.QueryAsync(contactsDatabaseId, new DatabasesQueryParameters()
        {
            Filter = new EmailFilter("Email empresa", contains: email)
        });

        if (results.Results.Count == 0)
        {
            return NotFound($"Contact {email} no encontrado");
        }
        else if (results.Results.Count > 1)
        {
            return Conflict($"Multiples contactos para {email} encontrados ({results.Results.Count} resultados)");
        }
        else
        {
            var resultId = results.Results.First().Id;
            var pageExport = await notionClient.ExportPageToJson(resultId);
            return Ok(pageExport);
        }
    }
}
