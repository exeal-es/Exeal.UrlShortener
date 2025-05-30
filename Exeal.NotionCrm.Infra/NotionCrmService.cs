using Exeal.UrlShortener.Api.Services;
using Notion.Client;

namespace Exeal.NotionCrm.Infra;

public class NotionCrmService(NotionClient notionClient)
{
    private const string ContactsDatabaseId = "ce55f603a4c54881bee92f4fee5eef42";

    public async Task<IEnumerable<NotionContactDto>> GetAllContacts()
    {
        var results = await notionClient.Databases.QueryAsync(ContactsDatabaseId, All());

        return results.Results.Select(Notion2Json.ExportContactPageToJson).ToList();
    }

    public async Task<IEnumerable<NotionContactDto>> GetContactInfoByWorkEmail(string email)
    {
        var results = await notionClient.Databases.QueryAsync(ContactsDatabaseId, ByEmailEmpresa(email));

        return results.Results.Select(Notion2Json.ExportContactPageToJson).ToList();
    }

    private static DatabasesQueryParameters All() => new();

    private static DatabasesQueryParameters ByEmailEmpresa(string email) =>
        new()
        {
            Filter = new EmailFilter("Email empresa", contains: email)
        };
}