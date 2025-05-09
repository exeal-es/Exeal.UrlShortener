using Exeal.UrlShortener.Ports.Output;

namespace Exeal.UrlShortener.Tests.Fakes;

public class StaticClock : IClock
{
    private readonly DateTime utcNow;

    public StaticClock()
    {
        utcNow = DateTime.UtcNow;
    }

    public DateTime UtcNow()
    {
        return utcNow;
    }
}