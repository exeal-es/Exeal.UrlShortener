using CSharpFunctionalExtensions;
using Exeal.NotionCrm.Infra.Ports;
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

    public async Task<Result<string>> CreateContact(CreateNotionContactDto request)
    {
        try
        {
            var properties = new Dictionary<string, PropertyValue>();
            
            if (request.Name != null)
                properties.Add("Name", Title(request.Name));
            if (request.PerfilDeLinkedin != null)
                properties.Add("Perfil de LinkedIn", Url(request.PerfilDeLinkedin));
            if (request.Cargo != null)
                properties.Add("Cargo", RichText(request.Cargo));
            if (request.Ciudad != null)
                properties.Add("Ciudad", Select(request.Ciudad));
            if (request.Telefono != null)
                properties.Add("Teléfono", PhoneNumber(request.Telefono));
            if (request.EmailPersonal != null)
                properties.Add("Email personal", Email(request.EmailPersonal));
            if (request.EmailEmpresa != null)
                properties.Add("Email empresa", Email(request.EmailEmpresa));
            properties.Add("Decision maker", Checkbox(request.DecisionMaker));
            
            var createParams = new PagesCreateParameters
            {
                Parent = new DatabaseParentInput { DatabaseId = ContactsDatabaseId },
                Properties = properties
            };

            var page = await notionClient.Pages.CreateAsync(createParams);
            return page.Id;
        }
        catch (NotionApiException e)
        {
            return Result.Failure<string>($"Failed to create contact: {e.Message}");
        }
    }

    private static CheckboxPropertyValue Checkbox(bool value)
    {
        return new CheckboxPropertyValue{ Checkbox = value };
    }

    private static EmailPropertyValue Email(string? value)
    {
        return new EmailPropertyValue { Email = value };
    }

    private static PhoneNumberPropertyValue PhoneNumber(string? value)
    {
        return new PhoneNumberPropertyValue { PhoneNumber = value };
    }

    private static SelectPropertyValue Select(string? value) => new() { Select = new SelectOption { Name = value } };

    private static TitlePropertyValue Title(string? value) => new() { Title = [RichTextText(value)] };

    private static UrlPropertyValue Url(string? value) => new() { Url = value };

    private static RichTextPropertyValue RichText(string? value) =>
        new()
        {
            RichText = new List<RichTextBase>
            {
                RichTextText(value)
            }
        };

    private static RichTextText RichTextText(string? value) =>
        new()
        {
            Text = new Text
            {
                Content = value ?? string.Empty
            }
        };
}
