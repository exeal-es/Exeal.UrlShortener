using CSharpFunctionalExtensions;

namespace Exeal.UrlShortener.Ports.Input;

public interface IShortUrlManager
{
    /// <summary>
    /// Creates a new short URL for the given destination URL. Optionally, a custom slug can be provided.
    /// </summary>
    /// <param name="destinationUrl">The destination URL to be shortened.</param>
    /// <param name="customSlug">An optional custom slug for the short URL. If not provided, a slug will be generated.</param>
    /// <returns>A result containing the generated slug if successful, or an error message if the operation fails.</returns>
    Task<Result<string>> CreateAsync(string destinationUrl, string? customSlug = null, string? title = null);
    
    /// <summary>
    /// Retrieves statistics for a short URL identified by its slug.
    /// </summary>
    /// <param name="slug">The unique identifier for the short URL.</param>
    /// <returns>A result containing the statistics of the short URL if the slug exists, or an error message if it does not.</returns>
    Task<Result<ShortUrlStats>> GetStatsAsync(string slug);

    /// <summary>
    /// Lists all created short URLs with pagination support.
    /// </summary>
    /// <param name="skip">The number of items to skip (for pagination).</param>
    /// <param name="take">The maximum number of items to return (for pagination).</param>
    /// <returns>A result containing a collection of short URLs.</returns>
    Task<Result<IEnumerable<ShortUrlDto>>> ListAsync(int skip = 0, int take = 10);
}
