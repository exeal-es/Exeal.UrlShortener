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
    string? LastEditedTime,
    string? DecisionMaker,
    string? CreatedTime,
    string? Name,
    string? UltimaInteraccion,
    string? PerfilDeLinkedin,
    string? Cargo
);
