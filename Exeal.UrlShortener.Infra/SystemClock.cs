using Exeal.UrlShortener.Ports.Output;

namespace Exeal.UrlShortener.Infra;

public class SystemClock : IClock
{
    public DateTime UtcNow()
    {
        return DateTime.UtcNow;
    }
}