using Exeal.UrlShortener.Ports.Input;
using Exeal.UrlShortener.Ports.Output;

namespace Exeal.UrlShortener.Application;

public class ShortUrlManager(
    IShortUrlRepository repository,
    ISlugGenerator slugGenerator,
    IClickTracker clickTracker,
    IClock clock) : IShortUrlManager
{
    public async Task<string> CreateAsync(string destinationUrl, string? customSlug = null)
    {
        var slug = customSlug ?? await slugGenerator.GenerateAsync();

        if (await repository.ExistsAsync(slug))
        {
            throw new SlugAlreadyExistsException(slug);
        }
        
        var shortUrl = new ShortUrl(slug, destinationUrl, clock.UtcNow());
        await repository.SaveAsync(shortUrl);

        return slug;
    }

    public async Task<ShortUrlStats> GetStatsAsync(string slug)
    {
        var shortUrl = await repository.LoadBySlugAsync(slug);

        if (shortUrl == null)
        {
            throw new SlugDoesNotExistException(slug);
        }
        
        return new ShortUrlStats(
            slug,
            shortUrl.DestinationUrl,
            shortUrl.CreatedAt,
            await clickTracker.GetClickCountAsync(slug),
            await clickTracker.GetUniqueVisitorCountAsync(slug));
    }
}