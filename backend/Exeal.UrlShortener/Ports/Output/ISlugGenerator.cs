namespace Exeal.UrlShortener.Ports.Output;

public interface ISlugGenerator
{
    Task<string> GenerateAsync();
}
