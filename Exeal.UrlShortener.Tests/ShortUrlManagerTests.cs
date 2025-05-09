using Exeal.UrlShortener.Application;
using Exeal.UrlShortener.Ports.Input;
using Exeal.UrlShortener.Ports.Output;
using Exeal.UrlShortener.Tests.Fakes;
using NSubstitute;

namespace Exeal.UrlShortener.Tests;

public class ShortUrlManagerTests
{
    private readonly ISlugGenerator slugGenerator;
    private readonly IShortUrlRepository shortUrlRepository;
    private readonly IClickTracker clickTracker;
    private readonly IClock clock;
    
    private readonly ShortUrlManager shortUrlManager;


    public ShortUrlManagerTests()
    {
        slugGenerator = Substitute.For<ISlugGenerator>();
        shortUrlRepository = new InMemoryShortUrlRepository();
        clickTracker = Substitute.For<IClickTracker>();
        clock = new StaticClock();

        shortUrlManager = new ShortUrlManager(shortUrlRepository, slugGenerator, clickTracker, clock);
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
    
    [Fact]
    public async Task CreateAsync_ShouldThrowSlugAlreadyExistsException_WhenSlugAlreadyExists()
    {
        // Arrange
        var validUrl = "https://example.com";
        var existingSlug = "curso-tdd";

        await shortUrlRepository.SaveAsync(new ShortUrl(existingSlug, validUrl, clock.UtcNow()));

        // Act
        var act = async () => await shortUrlManager.CreateAsync(validUrl, existingSlug);

        // Assert
        await Assert.ThrowsAsync<SlugAlreadyExistsException>(act);
    }
    
    [Fact]
    public async Task GetStatsAsync_ShouldReturnStatsWithTotalClicksAndUniqueVisitors_WhenSlugExists()
    {
        // Arrange
        var slug = "curso-tdd";
        var totalClicks = 10;
        var uniqueVisitors = 5;

        var shortUrl = new ShortUrl(slug, "https://example.com", clock.UtcNow());
        await shortUrlRepository.SaveAsync(shortUrl);

        clickTracker.GetClickCountAsync(slug).Returns(totalClicks);
        clickTracker.GetUniqueVisitorCountAsync(slug).Returns(uniqueVisitors);

        // Act
        var stats = await shortUrlManager.GetStatsAsync(slug);

        // Assert
        Assert.NotNull(stats);
        Assert.Equal(totalClicks, stats.ClickCount);
        Assert.Equal(uniqueVisitors, stats.UniqueVisitorCount);
    }
}