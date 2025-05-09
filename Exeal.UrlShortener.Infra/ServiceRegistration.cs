using Exeal.UrlShortener.Ports.Output;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Exeal.UrlShortener.Infra;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        
        services.AddScoped<IShortUrlRepository>(sp => 
            new PostgresShortUrlRepository(configuration.GetConnectionString("DefaultConnection")!));

        services.AddScoped<IClickTracker>(sp => 
            new PostgresClickTracker(configuration.GetConnectionString("DefaultConnection")!));

        services.AddScoped<ISlugGenerator, RandomSlugGenerator>();
        services.AddScoped<IClock, SystemClock>();
        
        return services;
    }
} 