namespace Exeal.UrlShortener.Api.Services;

public record NotionContactDto(
    string Id,
    string? EsFueAlumnoEn,
    string? Ciudad,
    string? Telefono,
    string? EmailPersonal,
    string? EmailEmpresa,
    string? Tags,
    string? Empresa,
    DateTime LastEditedTime,
    string? DecisionMaker,
    DateTime CreatedTime,
    string? Name,
    DateTime? UltimaInteraccion,
    string? PerfilDeLinkedin,
    string? Cargo
);
