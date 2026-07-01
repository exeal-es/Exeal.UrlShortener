using Exeal.UrlShortener.Ports.Output;

namespace Exeal.UrlShortener.Tests.Fakes;

public class StaticClock : IClock
{
    private readonly DateTime utcNow = DateTime.UtcNow;

    public DateTime UtcNow()
    {
        return utcNow;
    }
}