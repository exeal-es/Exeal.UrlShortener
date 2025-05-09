namespace Exeal.UrlShortener.Ports.Input;

public record ShortUrlStats
(
    string Slug,
    string DestinationUrl,
    DateTime CreatedAt,
    int ClickCount,
    int UniqueVisitorCount
);
