namespace Exeal.UrlShortener.Ports.Input;

public interface IShortUrlResolver
{
    Task<string> ResolveAsync(string slug, string ipAddress, string userAgent);
}