using Exeal.UrlShortener.Application;
using Exeal.UrlShortener.Ports.Output;
using Exeal.UrlShortener.Tests.Fakes;
using Microsoft.Extensions.Logging.Abstractions;

namespace Exeal.UrlShortener.Tests;

public class ShortUrlManagerTests
{
    private const string GeneratedSlug = "abc123";
    private const string TestBaseUrl = "https://test.short";

    private readonly ISlugGenerator slugGenerator;
    private readonly IShortUrlRepository shortUrlRepository;
    private readonly InMemoryClickTrackingService clickTrackingService;
    private readonly IClock clock;
    private readonly IUrlConfiguration urlConfiguration;

    private readonly ShortUrlManager shortUrlManager;

    public ShortUrlManagerTests()
    {
        slugGenerator = new StaticSlugGenerator(GeneratedSlug);
        shortUrlRepository = new InMemoryShortUrlRepository();
        clickTrackingService = new InMemoryClickTrackingService();
        clock = new StaticClock();
        urlConfiguration = new StaticUrlConfiguration(TestBaseUrl);

        shortUrlManager = new ShortUrlManager(NullLogger<ShortUrlManager>.Instance, shortUrlRepository, slugGenerator,
            clickTrackingService, urlConfiguration, clock);
    }

    [Fact]
    public async Task CreateAsync_ShouldGenerateSlugAndSaveShortUrl_WhenCalledWithValidUrlAndNoSlug()
    {
        // Arrange
        var validUrl = "https://example.com";

        // Act
        var result = await shortUrlManager.CreateAsync(validUrl);

        // Assert
        Assert.Equal(GeneratedSlug, result);

        var exists = await shortUrlRepository.ExistsAsync(GeneratedSlug);
        Assert.True(exists);

        var shortUrl = await shortUrlRepository.LoadBySlugAsync(GeneratedSlug);
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
    public async Task CreateAsync_ShouldSaveTitle_WhenTitleIsProvided()
    {
        // Arrange
        var validUrl = "https://example.com";
        var title = "Example Site";

        // Act
        await shortUrlManager.CreateAsync(validUrl, title: title);

        // Assert
        var shortUrl = await shortUrlRepository.LoadBySlugAsync(GeneratedSlug);
        Assert.NotNull(shortUrl);
        Assert.Equal(title, shortUrl.Title);
    }

    [Fact]
    public async Task CreateAsync_ShouldSaveNullTitle_WhenNoTitleIsProvided()
    {
        // Arrange
        var validUrl = "https://example.com";

        // Act
        await shortUrlManager.CreateAsync(validUrl);

        // Assert
        var shortUrl = await shortUrlRepository.LoadBySlugAsync(GeneratedSlug);
        Assert.NotNull(shortUrl);
        Assert.Null(shortUrl.Title);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenSlugAlreadyExists()
    {
        // Arrange
        var validUrl = "https://example.com";
        var existingSlug = "curso-tdd";

        await shortUrlRepository.SaveAsync(new ShortUrl(existingSlug, validUrl, clock.UtcNow()));

        // Act
        var result = await shortUrlManager.CreateAsync(validUrl, existingSlug);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal($"The slug {existingSlug} is already exists.", result.Error);
    }

    [Fact]
    public async Task GetStatsAsync_ShouldReturnStatsWithTotalClicksAndUniqueVisitors_WhenSlugExists()
    {
        // Arrange
        var slug = "curso-tdd";
        var ipAddress = "127.0.0.1";
        var userAgent = "TestUserAgent";

        var shortUrl = new ShortUrl(slug, "https://example.com", clock.UtcNow());
        await shortUrlRepository.SaveAsync(shortUrl);

        await clickTrackingService.RegisterAsync(slug, "1.2.3.4", userAgent);
        await clickTrackingService.RegisterAsync(slug, "1.2.3.4", userAgent);
        await clickTrackingService.RegisterAsync(slug, "5.6.7.8", userAgent);

        // Act
        var result = await shortUrlManager.GetStatsAsync(slug);

        // Assert
        Assert.True(result.IsSuccess);
        var stats = result.Value;
        Assert.Equal(3, stats.ClickCount);
        Assert.Equal(2, stats.UniqueVisitorCount);
        Assert.Equal($"{TestBaseUrl}/{slug}", stats.FullUrl);
    }

    [Fact]
    public async Task GetStatsAsync_ShouldIncludeTitle_WhenTitleIsSet()
    {
        // Arrange
        var slug = "curso-tdd";
        var title = "TDD Course";
        var shortUrl = new ShortUrl(slug, "https://example.com", clock.UtcNow(), title);
        await shortUrlRepository.SaveAsync(shortUrl);

        // Act
        var result = await shortUrlManager.GetStatsAsync(slug);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(title, result.Value.Title);
    }

    [Fact]
    public async Task GetStatsAsync_ShouldFail_WhenSlugDoesNotExist()
    {
        // Arrange
        var nonExistentSlug = "non-existent-slug";

        // Act
        var result = await shortUrlManager.GetStatsAsync(nonExistentSlug);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal($"The slug {nonExistentSlug} does not exist.", result.Error);
    }

    [Fact]
    public async Task ListAsync_ShouldReturnShortUrls_WhenUrlsExist()
    {
        // Arrange
        var shortUrls = new[]
        {
            new ShortUrl("slug1", "https://example1.com", clock.UtcNow()),
            new ShortUrl("slug2", "https://example2.com", clock.UtcNow()),
            new ShortUrl("slug3", "https://example3.com", clock.UtcNow())
        };

        foreach (var shortUrl in shortUrls)
        {
            await shortUrlRepository.SaveAsync(shortUrl);
        }

        // Act
        var result = await shortUrlManager.ListAsync();

        // Assert
        Assert.True(result.IsSuccess);
        var returnedUrls = result.Value.ToList();
        Assert.Equal(3, returnedUrls.Count);

        // Verify URLs
        var firstUrl = returnedUrls.First(s => s.Slug == "slug1");
        Assert.Equal("https://example1.com", firstUrl.DestinationUrl);

        var secondUrl = returnedUrls.First(s => s.Slug == "slug2");
        Assert.Equal("https://example2.com", secondUrl.DestinationUrl);

        var thirdUrl = returnedUrls.First(s => s.Slug == "slug3");
        Assert.Equal("https://example3.com", thirdUrl.DestinationUrl);
    }

    [Fact]
    public async Task ListAsync_ShouldRespectPagination_WhenUrlsExist()
    {
        // Arrange
        var shortUrls = new[]
        {
            new ShortUrl("slug1", "https://example1.com", clock.UtcNow()),
            new ShortUrl("slug2", "https://example2.com", clock.UtcNow()),
            new ShortUrl("slug3", "https://example3.com", clock.UtcNow()),
            new ShortUrl("slug4", "https://example4.com", clock.UtcNow()),
            new ShortUrl("slug5", "https://example5.com", clock.UtcNow())
        };

        foreach (var shortUrl in shortUrls)
        {
            await shortUrlRepository.SaveAsync(shortUrl);
        }

        // Act
        var result = await shortUrlManager.ListAsync(skip: 2, take: 2);

        // Assert
        Assert.True(result.IsSuccess);
        var returnedUrls = result.Value.ToList();
        Assert.Equal(2, returnedUrls.Count);
        Assert.Equal("slug3", returnedUrls[0].Slug);
        Assert.Equal("slug4", returnedUrls[1].Slug);
    }

    [Fact]
    public async Task ListAsync_ShouldReturnEmptyList_WhenNoUrlsExist()
    {
        // Act
        var result = await shortUrlManager.ListAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task ListAsync_ShouldIncludeTitle_WhenTitleIsSet()
    {
        // Arrange
        var shortUrls = new[]
        {
            new ShortUrl("slug1", "https://example1.com", clock.UtcNow(), "First"),
            new ShortUrl("slug2", "https://example2.com", clock.UtcNow(), null),
            new ShortUrl("slug3", "https://example3.com", clock.UtcNow(), "Third")
        };

        foreach (var shortUrl in shortUrls)
        {
            await shortUrlRepository.SaveAsync(shortUrl);
        }

        // Act
        var result = await shortUrlManager.ListAsync();

        // Assert
        Assert.True(result.IsSuccess);
        var returnedUrls = result.Value.ToList();
        Assert.Equal("First", returnedUrls.First(s => s.Slug == "slug1").Title);
        Assert.Null(returnedUrls.First(s => s.Slug == "slug2").Title);
        Assert.Equal("Third", returnedUrls.First(s => s.Slug == "slug3").Title);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateDestinationUrl_WhenSlugExists()
    {
        // Arrange
        var slug = "curso-tdd";
        var originalUrl = "https://example.com";
        var newUrl = "https://updated.com";
        await shortUrlRepository.SaveAsync(new ShortUrl(slug, originalUrl, clock.UtcNow()));

        // Act
        var result = await shortUrlManager.UpdateAsync(slug, newUrl, null);

        // Assert
        Assert.True(result.IsSuccess);
        var shortUrl = await shortUrlRepository.LoadBySlugAsync(slug);
        Assert.NotNull(shortUrl);
        Assert.Equal(newUrl, shortUrl.DestinationUrl);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTitle_WhenSlugExists()
    {
        // Arrange
        var slug = "curso-tdd";
        await shortUrlRepository.SaveAsync(new ShortUrl(slug, "https://example.com", clock.UtcNow(), "Old Title"));

        // Act
        var result = await shortUrlManager.UpdateAsync(slug, null, "New Title");

        // Assert
        Assert.True(result.IsSuccess);
        var shortUrl = await shortUrlRepository.LoadBySlugAsync(slug);
        Assert.NotNull(shortUrl);
        Assert.Equal("New Title", shortUrl.Title);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateBothFields_WhenBothProvided()
    {
        // Arrange
        var slug = "curso-tdd";
        await shortUrlRepository.SaveAsync(new ShortUrl(slug, "https://example.com", clock.UtcNow(), "Old Title"));

        // Act
        var result = await shortUrlManager.UpdateAsync(slug, "https://updated.com", "New Title");

        // Assert
        Assert.True(result.IsSuccess);
        var shortUrl = await shortUrlRepository.LoadBySlugAsync(slug);
        Assert.NotNull(shortUrl);
        Assert.Equal("https://updated.com", shortUrl.DestinationUrl);
        Assert.Equal("New Title", shortUrl.Title);
    }

    [Fact]
    public async Task UpdateAsync_ShouldNotChangeFields_WhenNullValuesProvided()
    {
        // Arrange
        var slug = "curso-tdd";
        var originalUrl = "https://example.com";
        var originalTitle = "Original Title";
        await shortUrlRepository.SaveAsync(new ShortUrl(slug, originalUrl, clock.UtcNow(), originalTitle));

        // Act
        var result = await shortUrlManager.UpdateAsync(slug, null, null);

        // Assert
        Assert.True(result.IsSuccess);
        var shortUrl = await shortUrlRepository.LoadBySlugAsync(slug);
        Assert.NotNull(shortUrl);
        Assert.Equal(originalUrl, shortUrl.DestinationUrl);
        Assert.Equal(originalTitle, shortUrl.Title);
    }

    [Fact]
    public async Task UpdateAsync_ShouldFail_WhenSlugDoesNotExist()
    {
        // Arrange
        var nonExistentSlug = "non-existent-slug";

        // Act
        var result = await shortUrlManager.UpdateAsync(nonExistentSlug, "https://updated.com", "New Title");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal($"The slug {nonExistentSlug} does not exist.", result.Error);
    }

    [Fact]
    public async Task ListAsync_ShouldReturnShortUrls_WithFullUrlFieldSet()
    {
        // Arrange
        var shortUrls = new[]
        {
            new ShortUrl("slug1", "https://example1.com", clock.UtcNow()),
            new ShortUrl("slug2", "https://example2.com", clock.UtcNow()),
            new ShortUrl("slug3", "https://example3.com", clock.UtcNow())
        };

        foreach (var shortUrl in shortUrls)
        {
            await shortUrlRepository.SaveAsync(shortUrl);
        }

        // Act
        var result = await shortUrlManager.ListAsync();

        // Assert
        Assert.True(result.IsSuccess);
        var returnedUrls = result.Value.ToList();
        Assert.Equal(3, returnedUrls.Count);
        foreach (var dto in returnedUrls)
        {
            Assert.Equal($"{TestBaseUrl}/{dto.Slug}", dto.FullUrl);
        }
    }
}