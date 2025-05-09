namespace Exeal.UrlShortener.Ports.Input;

public class SlugAlreadyExistsException(string Slug) : Exception;