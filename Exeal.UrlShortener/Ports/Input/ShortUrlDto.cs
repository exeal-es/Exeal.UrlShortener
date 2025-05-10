namespace Exeal.UrlShortener.Ports.Input;

public record ShortUrlDto
(
    string Slug,
    string DestinationUrl,
    DateTime CreatedAt,
    string FullUrl
);