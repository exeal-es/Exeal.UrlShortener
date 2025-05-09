namespace Exeal.UrlShortener.Ports.Input;

public interface IShortUrlManager
{
    Task<string> CreateAsync(string destinationUrl, string? customSlug = null);
    Task<ShortUrlStats> GetStatsAsync(string slug);
}
