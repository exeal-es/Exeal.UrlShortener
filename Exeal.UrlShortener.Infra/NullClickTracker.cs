using Exeal.UrlShortener.Ports.Output;

namespace Exeal.UrlShortener.Infra;

public class NullClickTracker : IClickTracker
{
    public Task RegisterAsync(string slug, string ipAddress, string userAgent)
    {
        return Task.CompletedTask;
    }

    public Task<int> GetClickCountAsync(string slug)
    {
        return Task.FromResult(0);
    }

    public Task<int> GetUniqueVisitorCountAsync(string slug)
    {
        return Task.FromResult(0);
    }
} 