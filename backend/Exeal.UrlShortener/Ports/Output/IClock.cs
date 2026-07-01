namespace Exeal.UrlShortener.Ports.Output;

public interface IClock
{
    DateTime UtcNow();
}
