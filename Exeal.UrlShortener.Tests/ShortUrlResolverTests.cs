using Exeal.UrlShortener.Application;
using Exeal.UrlShortener.Ports.Output;
using Exeal.UrlShortener.Tests.Fakes;
using NSubstitute;

namespace Exeal.UrlShortener.Tests;

public class ShortUrlResolverTests
{
    private readonly ISlugGenerator slugGenerator;
    private readonly IShortUrlRepository shortUrlRepository;
    private readonly IClickTracker clickTracker;
    private readonly IClock clock;
    
    private readonly ShortUrlResolver shortUrlResolver;

    public ShortUrlResolverTests()
    {
        slugGenerator = Substitute.For<ISlugGenerator>();
        shortUrlRepository = new InMemoryShortUrlRepository();
        clickTracker = Substitute.For<IClickTracker>();
        clock = new StaticClock();

        shortUrlResolver = new ShortUrlResolver(shortUrlRepository, slugGenerator, clickTracker, clock);
    }
    
    [Fact]
    public async Task ResolveAsync_ShouldReturnDestinationUrlAndTrackClick_WhenSlugExists()
    {
        // Arrange
        var slug = "abc123";
        var destinationUrl = "https://example.com";
        var ip = "192.168.1.1";
        var userAgent = "Mozilla/5.0";

        var shortUrl = new ShortUrl(slug, destinationUrl, clock.UtcNow());
        await shortUrlRepository.SaveAsync(shortUrl);

        // Act
        var result = await shortUrlResolver.ResolveAsync(slug, ip, userAgent);

        // Assert
        Assert.Equal(destinationUrl, result);
        await clickTracker.Received(1).RegisterAsync(slug, ip, userAgent);
    }
}