using CSharpFunctionalExtensions;
using Exeal.UrlShortener.Ports.Input;
using Exeal.UrlShortener.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Exeal.UrlShortener.Application;

public class ShortUrlResolver(
    ILogger<ShortUrlResolver> logger,
    IShortUrlRepository repository,
    IClickTracker clickTracker) : IShortUrlResolver
{
    public async Task<Result<string>> ResolveAsync(string slug, string ipAddress, string userAgent)
    {
        logger.LogInformation("ResolveAsync - Resolving slug {Slug}", slug);
        var shortUrl = await repository.LoadBySlugAsync(slug);

        if (shortUrl == null)
        {
            logger.LogWarning("ResolveAsync - Slug {Slug} does not exist", slug);
            return Result.Failure<string>($"The slug {slug} does not exist.");
        }

        await clickTracker.RegisterAsync(slug, ipAddress, userAgent);

        logger.LogInformation("ResolveAsync - Successfully resolved slug {Slug} to {DestinationUrl}", slug,
            shortUrl.DestinationUrl);
        return shortUrl.DestinationUrl;
    }
}