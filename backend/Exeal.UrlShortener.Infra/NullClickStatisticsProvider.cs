using Exeal.UrlShortener.Ports.Output;

namespace Exeal.UrlShortener.Infra;

public class NullClickStatisticsProvider : IClickStatisticsProvider
{
    public Task<int> GetClickCountAsync(string slug)
    {
        return Task.FromResult(0);
    }

    public Task<int> GetUniqueVisitorCountAsync(string slug)
    {
        return Task.FromResult(0);
    }
}