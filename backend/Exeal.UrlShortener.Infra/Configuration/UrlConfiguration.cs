using Exeal.UrlShortener.Ports.Output;
using Microsoft.Extensions.Configuration;

namespace Exeal.UrlShortener.Infra.Configuration;

public class UrlConfiguration : IUrlConfiguration
{
    private readonly IConfiguration _configuration;

    public UrlConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string BaseUrl => _configuration["UrlShortener:BaseUrl"] ?? throw new InvalidOperationException("BaseUrl configuration is missing");
} 