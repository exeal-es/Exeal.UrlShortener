using CSharpFunctionalExtensions;
using Exeal.UrlShortener.Api.Services;
using Notion.Client;

namespace Exeal.NotionCrm.Infra;

public class NotionCrmService(NotionClient notionClient)
{
    private const string ContactsDatabaseId = "ce55f603a4c54881bee92f4fee5eef42";

    public async Task<IEnumerable<NotionContactDto>> FindContacts(string? email = null)
    {
        var queryParameters = email == null
            ? All()
            : ByEmail(email);

        var results = await notionClient.Databases.QueryAsync(ContactsDatabaseId, queryParameters);

        return results.Results.Select(Notion2Json.ExportContactPageToJson).ToList();
    }

    private static DatabasesQueryParameters All() => new();

    private static DatabasesQueryParameters ByEmail(string email) =>
        new()
        {
            Filter = new CompoundFilter(
                or:
                [
                    new EmailFilter("Email empresa", contains: email),
                    new EmailFilter("Email personal", contains: email)
                ])
        };

    public async Task<Result<NotionContactDto>> GetContactById(Guid id)
    {
        try
        {
            var page = await notionClient.Pages.RetrieveAsync(id.ToString());
            return Notion2Json.ExportContactPageToJson(page);
        }
        catch (NotionApiException e)
        {
            return Result.Failure<NotionContactDto>($"Contact with ID {id} not found: {e.Message}");
        }
    }
}