using Exeal.UrlShortener.Ports.Output;

namespace Exeal.UrlShortener.Tests.Fakes;

public class InMemoryShortUrlRepository : IShortUrlRepository
{
    private readonly Dictionary<string, ShortUrl> storage = new();
    
    public Task<bool> ExistsAsync(string slug)
    {
        return Task.FromResult(storage.ContainsKey(slug));
    }

    public Task SaveAsync(ShortUrl shortUrl)
    {
        if (!storage.TryAdd(shortUrl.Slug, shortUrl))
        {
            throw new InvalidOperationException("Slug already exists.");
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(ShortUrl shortUrl)
    {
        if (!storage.ContainsKey(shortUrl.Slug))
            throw new InvalidOperationException("Slug not found.");
        storage[shortUrl.Slug] = shortUrl;
        return Task.CompletedTask;
    }

    public Task<ShortUrl?> LoadBySlugAsync(string slug)
    {
        if (storage.TryGetValue(slug, out var shortUrl))
        {
            return Task.FromResult<ShortUrl?>(shortUrl);
        }

        return Task.FromResult<ShortUrl?>(null);
    }

    public Task<IEnumerable<ShortUrl>> ListAsync(int skip = 0, int take = 10)
    {
        return Task.FromResult(storage.Values.Skip(skip).Take(take));
    }
}
