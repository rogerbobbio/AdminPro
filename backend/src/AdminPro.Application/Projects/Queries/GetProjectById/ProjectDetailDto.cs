using System;
using System.Collections.Generic;

namespace AdminPro.Application.Projects.Queries.GetProjectById;

public record ProjectDetailDto(
    int Id,
    string Nombre,
    string? Descripcion,
    bool Activo,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<BaseDeDatosDto> BasesDeDatos,
    IReadOnlyList<ApplicationSummaryDto> Applications);

public record BaseDeDatosDto(
    int Id,
    string Nombre,
    string? Servidor,
    int? DatabaseId,
    string? LoginName,
    string? Password,
    string? Ambiente,
    string? Notas,
    bool Activo);

public record ApplicationSummaryDto(
    int Id,
    string Nombre,
    string? TecnologiaFront,
    string? TecnologiaBack,
    int Orden,
    bool Activo);
