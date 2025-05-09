namespace Exeal.UrlShortener.Ports.Output;

public record ShortUrl
(
    string Slug,
    string DestinationUrl,
    DateTime CreatedAt
);
