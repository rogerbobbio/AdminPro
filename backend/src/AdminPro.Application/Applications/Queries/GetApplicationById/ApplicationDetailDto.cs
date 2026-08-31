using System;
using System.Collections.Generic;

namespace AdminPro.Application.Applications.Queries.GetApplicationById;

public record ApplicationDetailDto(
    int Id,
    int ProyectoId,
    string Nombre,
    string? Descripcion,
    string? TecnologiaFront,
    string? TecnologiaBack,
    string? RamaDesarrollo,
    string? ApplicationName,
    string? TieneProyectoBD,
    string? RutaLocal,
    string? RutaGit,
    string? ComoSeLevanta,
    string? NotasCompilacion,
    int Orden,
    bool Activo,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<AmbienteDto> Ambientes,
    IReadOnlyList<object> Reportes,
    IReadOnlyList<object> Notas,
    IReadOnlyList<object> Documentos,
    IReadOnlyList<object> FixDatas,
    IReadOnlyList<object> Servicios);

public record AmbienteDto(
    int Id,
    string Nombre,
    string? Url,
    bool EsWebApi,
    string? Notas,
    int Orden,
    bool Activo);
