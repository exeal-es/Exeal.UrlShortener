using Exeal.UrlShortener.Ports.Output;

namespace Exeal.UrlShortener.Infra;

public class RandomSlugGenerator : ISlugGenerator
{
    private const int SlugLength = 8;

    public Task<string> GenerateAsync()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var slug = new string(Enumerable.Repeat(chars, SlugLength)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        return Task.FromResult(slug);
    }
}