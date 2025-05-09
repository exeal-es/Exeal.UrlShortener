using Exeal.UrlShortener.Ports.Output;

namespace Exeal.UrlShortener.Infra;

public class CachedShortUrlRepository(IShortUrlRepository repository) : IShortUrlRepository
{
    private readonly Dictionary<string, ShortUrl> _cache = new();

    public async Task<bool> ExistsAsync(string slug)
    {
        return await repository.ExistsAsync(slug);
    }

    public async Task SaveAsync(ShortUrl shortUrl)
    {
        await repository.SaveAsync(shortUrl);
        _cache[shortUrl.Slug] = shortUrl;
    }

    public async Task<ShortUrl?> LoadBySlugAsync(string slug)
    {
        if (_cache.TryGetValue(slug, out var cachedUrl))
        {
            return cachedUrl;
        }

        var shortUrl = await repository.LoadBySlugAsync(slug);
        if (shortUrl != null)
        {
            _cache[slug] = shortUrl;
        }

        return shortUrl;
    }

    public Task<IEnumerable<ShortUrl>> ListAsync(int skip = 0, int take = 10)
    {
        return repository.ListAsync(skip, take);
    }
} 