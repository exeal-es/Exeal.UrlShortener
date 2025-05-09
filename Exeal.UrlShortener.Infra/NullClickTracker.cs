using Exeal.UrlShortener.Ports.Output;

namespace Exeal.UrlShortener.Infra;

public class NullClickTracker : IClickTracker
{
    public Task RegisterAsync(string slug, string ipAddress, string userAgent)
    {
        return Task.CompletedTask;
    }
} 