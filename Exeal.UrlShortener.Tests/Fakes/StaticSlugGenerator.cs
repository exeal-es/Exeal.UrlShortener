using Exeal.UrlShortener.Ports.Output;

namespace Exeal.UrlShortener.Tests.Fakes;

public class StaticSlugGenerator(string slug) : ISlugGenerator
{
    public Task<string> GenerateAsync()
    {
        return Task.FromResult(slug);
    }
}