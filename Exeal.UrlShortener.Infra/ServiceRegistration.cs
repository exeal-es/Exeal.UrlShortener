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
        {
            var postgresRepo = new PostgresShortUrlRepository(configuration.GetConnectionString("DefaultConnection")!);
            return new CachedShortUrlRepository(postgresRepo);
        });

        var isTrackingEnabled = configuration.GetValue<bool>("Features:Tracking:Enabled");
        if (isTrackingEnabled)
        {
            var postgresClickTrackingService = new PostgresClickTrackingService(configuration.GetConnectionString("DefaultConnection")!);
            services.AddScoped<IClickTracker>(_ => postgresClickTrackingService);
            services.AddScoped<IClickStatisticsProvider>(_ => postgresClickTrackingService);
        }
        else
        {
            services.AddScoped<IClickTracker, NullClickTracker>();
            services.AddScoped<IClickStatisticsProvider, NullClickStatisticsProvider>();

        }

        services.AddScoped<ISlugGenerator, RandomSlugGenerator>();
        services.AddScoped<IClock, SystemClock>();
        
        return services;
    }
} 