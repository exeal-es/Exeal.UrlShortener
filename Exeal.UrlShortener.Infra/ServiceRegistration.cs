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

        var isTrackingEnabled = configuration.GetValue<bool>("Features:Tracking:Enabled");
        services.AddScoped<IClickTracker>(sp => 
            isTrackingEnabled 
                ? new PostgresClickTracker(configuration.GetConnectionString("DefaultConnection")!)
                : new NullClickTracker());

        services.AddScoped<ISlugGenerator, RandomSlugGenerator>();
        services.AddScoped<IClock, SystemClock>();
        
        return services;
    }
} 