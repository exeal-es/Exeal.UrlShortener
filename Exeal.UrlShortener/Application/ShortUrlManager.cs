using Exeal.UrlShortener.Ports.Input;
using Exeal.UrlShortener.Ports.Output;

namespace Exeal.UrlShortener.Application;

public class ShortUrlManager(
    IShortUrlRepository repository, ISlugGenerator slugGenerator, IClock clock) : IShortUrlManager
{
    public async Task<string> CreateAsync(string destinationUrl, string? customSlug = null)
    {
        var slug = customSlug ?? await slugGenerator.GenerateAsync();

        var shortUrl = new ShortUrl(slug, destinationUrl, clock.UtcNow());
        await repository.SaveAsync(shortUrl);

        return slug;
    }

    public Task<ShortUrlStats> GetStatsAsync(string slug)
    {
        throw new NotImplementedException();
    }
}