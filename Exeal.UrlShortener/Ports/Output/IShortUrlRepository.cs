namespace Exeal.UrlShortener.Ports.Output;

public interface IShortUrlRepository
{
    Task<bool> ExistsAsync(string slug);
    Task SaveAsync(ShortUrl shortUrl);
    Task<ShortUrl?> LoadBySlugAsync(string slug);
    Task<IEnumerable<ShortUrl>> ListAsync(int skip = 0, int take = 10);
}