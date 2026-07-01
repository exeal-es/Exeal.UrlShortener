namespace Exeal.UrlShortener.Api.Models;

public record UpdateShortUrlRequest(string? DestinationUrl = null, string? Title = null);
