namespace Exeal.UrlShortener.Ports.Input;

public class SlugDoesNotExistException(string Slug) : Exception;