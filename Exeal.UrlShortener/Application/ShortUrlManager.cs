using CSharpFunctionalExtensions;
using Exeal.UrlShortener.Ports.Input;
using Exeal.UrlShortener.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Exeal.UrlShortener.Application;

public class ShortUrlManager(
    ILogger<ShortUrlManager> logger,
    IShortUrlRepository repository,
    ISlugGenerator slugGenerator,
    IClickStatisticsProvider clickStatisticsProvider,
    IClock clock) : IShortUrlManager
{
    public async Task<Result<string>> CreateAsync(string destinationUrl, string? customSlug = null)
    {
        logger.LogInformation("CreateAsync - Creating short URL for {DestinationUrl} with custom slug {CustomSlug}",
            destinationUrl, customSlug);

        var slug = customSlug ?? await slugGenerator.GenerateAsync();

        if (await repository.ExistsAsync(slug))
        {
            logger.LogInformation("CreateAsync - Tried to create a short URL with an existing slug {Slug}", slug);
            return Result.Failure<string>($"The slug {slug} is already exists.");
        }

        var shortUrl = new ShortUrl(slug, destinationUrl, clock.UtcNow());
        await repository.SaveAsync(shortUrl);

        logger.LogInformation("CreateAsync - Short URL created with slug {Slug} for {DestinationUrl}", slug,
            destinationUrl);
        return slug;
    }

    public async Task<Result<ShortUrlStats>> GetStatsAsync(string slug)
    {
        logger.LogInformation("GetStatsAsync - Fetching stats for slug {Slug}", slug);
        var shortUrl = await repository.LoadBySlugAsync(slug);

        if (shortUrl == null)
        {
            logger.LogWarning("GetStatsAsync - Slug {Slug} does not exist", slug);
            return Result.Failure<ShortUrlStats>($"The slug {slug} does not exist.");
        }

        var shortUrlStats = new ShortUrlStats(
            slug,
            shortUrl.DestinationUrl,
            shortUrl.CreatedAt,
            await clickStatisticsProvider.GetClickCountAsync(slug),
            await clickStatisticsProvider.GetUniqueVisitorCountAsync(slug));

        logger.LogInformation("GetStatsAsync - Completed fetching stats for slug {Slug}", slug);
        return shortUrlStats;
    }

    public async Task<Result<IEnumerable<ShortUrlDto>>> ListAsync(int skip = 0, int take = 10)
    {
        logger.LogInformation("ListAsync - Fetching short URLs with skip {Skip} and take {Take}", skip, take);

        try
        {
            var shortUrls = await repository.ListAsync(skip, take);
            var dtos = shortUrls.Select(url => new ShortUrlDto(url.Slug, url.DestinationUrl, url.CreatedAt));
            logger.LogInformation("ListAsync - Successfully fetched {Count} short URLs", shortUrls.Count());
            return Result.Success(dtos);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ListAsync - Error occurred while fetching short URLs");
            return Result.Failure<IEnumerable<ShortUrlDto>>("An error occurred while fetching the list of short URLs.");
        }
    }
}