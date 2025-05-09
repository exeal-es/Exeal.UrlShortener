using CSharpFunctionalExtensions;

namespace Exeal.UrlShortener.Ports.Input;

public interface IShortUrlResolver
{
    Task<Result<string>> ResolveAsync(string slug, string ipAddress, string userAgent);
}