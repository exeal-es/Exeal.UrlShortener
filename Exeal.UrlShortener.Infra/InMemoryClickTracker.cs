using Exeal.UrlShortener.Ports.Output;

namespace Exeal.UrlShortener.Infra;

public class InMemoryClickTracker : IClickTracker
{
    private readonly Dictionary<string, HashSet<string>> _uniqueVisitors = new();
    private readonly Dictionary<string, int> _clickCounts = new();

    public Task RegisterAsync(string slug, string ipAddress, string userAgent)
    {
        if (!_uniqueVisitors.ContainsKey(slug))
        {
            _uniqueVisitors[slug] = new HashSet<string>();
            _clickCounts[slug] = 0;
        }

        _uniqueVisitors[slug].Add(ipAddress);
        _clickCounts[slug]++;
        return Task.CompletedTask;
    }

    public Task<int> GetClickCountAsync(string slug)
    {
        _clickCounts.TryGetValue(slug, out var count);
        return Task.FromResult(count);
    }

    public Task<int> GetUniqueVisitorCountAsync(string slug)
    {
        _uniqueVisitors.TryGetValue(slug, out var visitors);
        return Task.FromResult(visitors?.Count ?? 0);
    }
}