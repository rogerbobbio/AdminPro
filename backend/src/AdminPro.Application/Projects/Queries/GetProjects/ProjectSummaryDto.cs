using System;

namespace AdminPro.Application.Projects.Queries.GetProjects;

public record ProjectSummaryDto(
    int Id,
    string Nombre,
    string? Descripcion,
    bool Activo,
    DateTime CreatedAt,
    DateTime UpdatedAt);
