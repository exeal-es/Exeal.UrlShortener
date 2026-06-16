using Exeal.UrlShortener.Ports.Output;
using NSubstitute;

namespace Exeal.UrlShortener.Infra.Tests;

public class CachedShortUrlRepositoryTests
{
    private readonly IShortUrlRepository _repository;
    private readonly CachedShortUrlRepository _cachedRepository;
    private readonly ShortUrl _testShortUrl;

    public CachedShortUrlRepositoryTests()
    {
        _repository = Substitute.For<IShortUrlRepository>();
        _cachedRepository = new CachedShortUrlRepository(_repository);
        _testShortUrl = new ShortUrl("test-slug", "https://example.com", DateTime.UtcNow, "Test Title");
    }

    [Fact]
    public async Task LoadBySlugAsync_ShouldReturnFromCache_WhenItemIsCached()
    {
        // Arrange
        _repository.LoadBySlugAsync(_testShortUrl.Slug).Returns(_testShortUrl);
        await _cachedRepository.LoadBySlugAsync(_testShortUrl.Slug); // First call to cache the item

        // Act
        var result = await _cachedRepository.LoadBySlugAsync(_testShortUrl.Slug);

        // Assert
        Assert.Equal(_testShortUrl, result);
        await _repository.Received(1).LoadBySlugAsync(_testShortUrl.Slug); // Should only be called once
    }

    [Fact]
    public async Task LoadBySlugAsync_ShouldReturnFromRepository_WhenItemIsNotCached()
    {
        // Arrange
        _repository.LoadBySlugAsync(_testShortUrl.Slug).Returns(_testShortUrl);

        // Act
        var result = await _cachedRepository.LoadBySlugAsync(_testShortUrl.Slug);

        // Assert
        Assert.Equal(_testShortUrl, result);
        await _repository.Received(1).LoadBySlugAsync(_testShortUrl.Slug);
    }

    [Fact]
    public async Task LoadBySlugAsync_ShouldNotCache_WhenRepositoryReturnsNull()
    {
        // Arrange
        _repository.LoadBySlugAsync(_testShortUrl.Slug).Returns((ShortUrl?)null);

        // Act
        var result = await _cachedRepository.LoadBySlugAsync(_testShortUrl.Slug);

        // Assert
        Assert.Null(result);
        await _repository.Received(1).LoadBySlugAsync(_testShortUrl.Slug);
    }

    [Fact]
    public async Task SaveAsync_ShouldUpdateCache_WhenSavingNewItem()
    {
        // Arrange
        _repository.LoadBySlugAsync(_testShortUrl.Slug).Returns(_testShortUrl);

        // Act
        await _cachedRepository.SaveAsync(_testShortUrl);
        var result = await _cachedRepository.LoadBySlugAsync(_testShortUrl.Slug);

        // Assert
        Assert.Equal(_testShortUrl, result);
        await _repository.Received(0).LoadBySlugAsync(_testShortUrl.Slug); // Should not call repository
    }

    [Fact]
    public async Task LoadBySlugAsync_ShouldPreserveTitle_WhenLoadingFromCache()
    {
        // Arrange
        _repository.LoadBySlugAsync(_testShortUrl.Slug).Returns(_testShortUrl);
        await _cachedRepository.LoadBySlugAsync(_testShortUrl.Slug); // populate cache

        // Act
        var result = await _cachedRepository.LoadBySlugAsync(_testShortUrl.Slug);

        // Assert
        Assert.Equal(_testShortUrl.Title, result!.Title);
    }

    [Fact]
    public async Task SaveAsync_ShouldPreserveTitle_WhenCachingItem()
    {
        // Act
        await _cachedRepository.SaveAsync(_testShortUrl);
        var result = await _cachedRepository.LoadBySlugAsync(_testShortUrl.Slug);

        // Assert
        Assert.Equal(_testShortUrl.Title, result!.Title);
    }

    [Fact]
    public async Task ExistsAsync_ShouldDelegateToRepository()
    {
        // Arrange
        _repository.ExistsAsync(_testShortUrl.Slug).Returns(true);

        // Act
        var result = await _cachedRepository.ExistsAsync(_testShortUrl.Slug);

        // Assert
        Assert.True(result);
        await _repository.Received(1).ExistsAsync(_testShortUrl.Slug);
    }
} 