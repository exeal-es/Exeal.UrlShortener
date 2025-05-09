namespace Exeal.UrlShortener.Ports.Output;

public interface IClickTracker
{
    Task RegisterAsync(string slug, string ipAddress, string userAgent);
    Task<int> GetClickCountAsync(string slug);
    Task<int> GetUniqueVisitorCountAsync(string slug);
}
