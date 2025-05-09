using Exeal.UrlShortener.Application;
using Exeal.UrlShortener.Ports.Output;
using Exeal.UrlShortener.Tests.Fakes;
using NSubstitute;

namespace Exeal.UrlShortener.Tests;

public class ShortUrlManagerTests
{
    private readonly ISlugGenerator slugGenerator;
    private readonly IShortUrlRepository shortUrlRepository;
    private readonly IClock clock;
    
    private readonly ShortUrlManager shortUrlManager;

    public ShortUrlManagerTests()
    {
        slugGenerator = Substitute.For<ISlugGenerator>();
        shortUrlRepository = new InMemoryShortUrlRepository();
        clock = new StaticClock();

        shortUrlManager = new ShortUrlManager(shortUrlRepository, slugGenerator, clock);
    }
    
    [Fact]
    public async Task CreateAsync_ShouldGenerateSlugAndSaveShortUrl_WhenCalledWithValidUrlAndNoSlug()
    {
        // Arrange
        var validUrl = "https://example.com";
        var generatedSlug = "abc123";

        slugGenerator.GenerateAsync().Returns(generatedSlug);
        
        // Act
        var result = await shortUrlManager.CreateAsync(validUrl);

        // Assert
        Assert.Equal(generatedSlug, result);
        
        var exists = await shortUrlRepository.ExistsAsync(generatedSlug);
        Assert.True(exists);
        
        var shortUrl = await shortUrlRepository.LoadBySlugAsync(generatedSlug);
        Assert.NotNull(shortUrl);
        Assert.Equal(validUrl, shortUrl.DestinationUrl);
        Assert.Equal(clock.UtcNow(), shortUrl.CreatedAt);
    }
    
    [Fact]
    public async Task CreateAsync_ShouldSaveShortUrlWithCustomSlug_WhenCalledWithValidUrlAndCustomSlug()
    {
        // Arrange
        var validUrl = "https://example.com";
        var customSlug = "pilares";

        // Act
        var result = await shortUrlManager.CreateAsync(validUrl, customSlug);

        // Assert
        Assert.Equal(customSlug, result);

        var exists = await shortUrlRepository.ExistsAsync(customSlug);
        Assert.True(exists);

        var shortUrl = await shortUrlRepository.LoadBySlugAsync(customSlug);
        Assert.NotNull(shortUrl);
        Assert.Equal(validUrl, shortUrl.DestinationUrl);
        Assert.Equal(clock.UtcNow(), shortUrl.CreatedAt);
    }
}
