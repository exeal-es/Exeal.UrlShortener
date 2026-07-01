using Exeal.UrlShortener.Ports.Output;

namespace Exeal.UrlShortener.Tests.Fakes;

public class StaticUrlConfiguration : IUrlConfiguration
{
    public string BaseUrl { get; }
    public StaticUrlConfiguration(string baseUrl)
    {
        BaseUrl = baseUrl;
    }
} 