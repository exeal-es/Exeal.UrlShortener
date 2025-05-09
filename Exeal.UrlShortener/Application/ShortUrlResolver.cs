using Exeal.UrlShortener.Ports.Input;
using Exeal.UrlShortener.Ports.Output;

namespace Exeal.UrlShortener.Application;

public class ShortUrlResolver(
    IShortUrlRepository repository, IClickTracker clickTracker) : IShortUrlResolver
{
    public async Task<string> ResolveAsync(string slug, string ipAddress, string userAgent)
    {
        var shortUrl = await repository.LoadBySlugAsync(slug);

        await clickTracker.RegisterAsync(slug, ipAddress, userAgent);
        return shortUrl.DestinationUrl;
    }
}
