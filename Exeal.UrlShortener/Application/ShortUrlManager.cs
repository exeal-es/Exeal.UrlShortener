using CSharpFunctionalExtensions;
using Exeal.UrlShortener.Ports.Input;
using Exeal.UrlShortener.Ports.Output;

namespace Exeal.UrlShortener.Application;

public class ShortUrlManager(
    IShortUrlRepository repository,
    ISlugGenerator slugGenerator,
    IClickTracker clickTracker,
    IClock clock) : IShortUrlManager
{
    public async Task<Result<string>> CreateAsync(string destinationUrl, string? customSlug = null)
    {
        var slug = customSlug ?? await slugGenerator.GenerateAsync();

        if (await repository.ExistsAsync(slug))
        {
            return Result.Failure<string>($"The slug {slug} is already exists.");
        }

        var shortUrl = new ShortUrl(slug, destinationUrl, clock.UtcNow());
        await repository.SaveAsync(shortUrl);

        return slug;
    }

    public async Task<Result<ShortUrlStats>> GetStatsAsync(string slug)
    {
        var shortUrl = await repository.LoadBySlugAsync(slug);

        if (shortUrl == null)
        {
            return Result.Failure<ShortUrlStats>($"The slug {slug} does not exist.");
        }

        return new ShortUrlStats(
            slug,
            shortUrl.DestinationUrl,
            shortUrl.CreatedAt,
            await clickTracker.GetClickCountAsync(slug),
            await clickTracker.GetUniqueVisitorCountAsync(slug));
    }
}