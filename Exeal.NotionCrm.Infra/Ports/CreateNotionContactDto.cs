namespace Exeal.NotionCrm.Infra.Ports;

public record CreateNotionContactDto(
    string? Name,
    string? PerfilDeLinkedin,
    string? Cargo,
    string? Ciudad,
    string? Telefono,
    string? EmailPersonal,
    string? EmailEmpresa,
    bool DecisionMaker
);
