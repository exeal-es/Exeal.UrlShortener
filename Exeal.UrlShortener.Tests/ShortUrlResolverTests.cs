using Exeal.UrlShortener.Application;
using Exeal.UrlShortener.Ports.Input;
using Exeal.UrlShortener.Ports.Output;
using Exeal.UrlShortener.Tests.Fakes;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Exeal.UrlShortener.Tests;

public class ShortUrlResolverTests
{
    private readonly IShortUrlRepository shortUrlRepository;
    private readonly IClickTracker clickTracker;

    private readonly ShortUrlResolver shortUrlResolver;

    public ShortUrlResolverTests()
    {
        shortUrlRepository = new InMemoryShortUrlRepository();
        clickTracker = Substitute.For<IClickTracker>();

        shortUrlResolver = new ShortUrlResolver(shortUrlRepository, clickTracker);
    }

    [Fact]
    public async Task ResolveAsync_ShouldReturnDestinationUrlAndTrackClick_WhenSlugExists()
    {
        // Arrange
        var slug = "abc123";
        var destinationUrl = "https://example.com";
        var ip = "192.168.1.1";
        var userAgent = "Mozilla/5.0";

        var shortUrl = new ShortUrl(slug, destinationUrl, DateTime.UtcNow);
        await shortUrlRepository.SaveAsync(shortUrl);

        // Act
        var result = await shortUrlResolver.ResolveAsync(slug, ip, userAgent);

        // Assert
        Assert.Equal(destinationUrl, result);
        await clickTracker.Received(1).RegisterAsync(slug, ip, userAgent);
    }

    [Fact]
    public async Task ResolveAsync_ShouldThrowSlugDoesNotExistException_WhenSlugDoesNotExist()
    {
        // Arrange
        var nonExistentSlug = "non-existent-slug";
        var ipAddress = "127.0.0.1";
        var userAgent = "TestUserAgent";

        // Act
        var act = async () => await shortUrlResolver.ResolveAsync(nonExistentSlug, ipAddress, userAgent);

        // Assert
        await Assert.ThrowsAsync<SlugDoesNotExistException>(act);
    }
}