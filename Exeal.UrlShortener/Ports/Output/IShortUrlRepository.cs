namespace Exeal.UrlShortener.Ports.Output;

public interface IShortUrlRepository
{
    Task<bool> ExistsAsync(string slug);
    Task SaveAsync(ShortUrl shortUrl);
    Task<ShortUrl?> LoadBySlugAsync(string slug);
}