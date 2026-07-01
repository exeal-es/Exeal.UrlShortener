using CSharpFunctionalExtensions;

namespace Exeal.UrlShortener.Ports.Input;

public interface IShortUrlResolver
{
    /// <summary>
    /// Resolves a short URL slug to its destination URL and tracks the click event.
    /// </summary>
    /// <param name="slug">The unique identifier for the short URL.</param>
    /// <param name="ipAddress">The IP address of the user accessing the short URL.</param>
    /// <param name="userAgent">The user agent string of the client accessing the short URL.</param>
    /// <returns>A result containing the destination URL if the slug exists, or an error message if it does not.</returns>
    Task<Result<string>> ResolveAsync(string slug, string ipAddress, string userAgent);
}