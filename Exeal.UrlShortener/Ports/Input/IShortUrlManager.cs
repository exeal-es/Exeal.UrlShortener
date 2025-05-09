using CSharpFunctionalExtensions;

namespace Exeal.UrlShortener.Ports.Input;

public interface IShortUrlManager
{
    Task<Result<string>> CreateAsync(string destinationUrl, string? customSlug = null);
    Task<Result<ShortUrlStats>> GetStatsAsync(string slug);
}
