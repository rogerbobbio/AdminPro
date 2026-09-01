namespace AdminPro.Application.Applications.Queries.GetApplicationsByProject;

public record ApplicationSummaryDto(
    int Id,
    string Nombre,
    string? TecnologiaFront,
    string? TecnologiaBack,
    int Orden,
    bool Activo);
