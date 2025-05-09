namespace Exeal.UrlShortener.Api.Models;

public record CreateShortUrlRequest(string DestinationUrl, string? CustomSlug = null); 