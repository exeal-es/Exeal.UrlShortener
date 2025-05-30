using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notion.Client;

namespace Exeal.NotionCrm.Infra;

public static class ServiceRegistration
{
    public static IServiceCollection AddNotionCrm(this IServiceCollection services, IConfiguration configuration)
    {
        var notionClient = NotionClientFactory.Create(new ClientOptions
        {
            AuthToken = configuration["NotionToken"]
        });
        services.AddSingleton(notionClient);
        
        services.AddScoped<NotionCrmService>();
        
        return services;
    }
}