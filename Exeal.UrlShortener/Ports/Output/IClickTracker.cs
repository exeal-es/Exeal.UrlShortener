namespace Exeal.UrlShortener.Ports.Output;

public interface IClickTracker
{
    Task RegisterAsync(string slug, string ipAddress, string userAgent);
}