namespace Exeal.UrlShortener.Ports.Output;

public interface IUrlConfiguration
{
    /// <summary>
    /// Gets the base URL for short URLs.
    /// </summary>
    string BaseUrl { get; }
} 