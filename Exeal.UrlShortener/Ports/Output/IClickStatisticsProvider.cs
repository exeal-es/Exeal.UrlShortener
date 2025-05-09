namespace Exeal.UrlShortener.Ports.Output;

public interface IClickStatisticsProvider
{
    Task<int> GetClickCountAsync(string slug);
    Task<int> GetUniqueVisitorCountAsync(string slug);
}