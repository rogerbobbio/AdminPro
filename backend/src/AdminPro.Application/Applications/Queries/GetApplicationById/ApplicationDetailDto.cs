using System;
using System.Collections.Generic;

namespace AdminPro.Application.Applications.Queries.GetApplicationById;

public record ApplicationDetailDto(
    int Id,
    int ProyectoId,
    string Nombre,
    string? Descripcion,
    string? Tipo,
    string? TecnologiaFront,
    string? TecnologiaBack,
    string? RamaDesarrollo,
    string? ApplicationName,
    string? RutaLocal,
    string? RutaGit,
    string? ComoSeLevanta,
    string? NotasCompilacion,
    int Orden,
    bool Activo,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<AmbienteDto> Ambientes,
    IReadOnlyList<ReporteDto> Reportes,
    IReadOnlyList<NotaDto> Notas,
    IReadOnlyList<DocumentoDto> Documentos,
    IReadOnlyList<FixDataDto> FixDatas,
    IReadOnlyList<object> Servicios);

public record AmbienteDto(
    int Id,
    string Nombre,
    string? Url,
    bool EsWebApi,
    string? Notas,
    int Orden,
    bool Activo);

public record ReporteDto(
    int Id,
    string ReportCode,
    string ReportName,
    string? RegionId,
    string? ReportPath,
    string? SpTranship,
    string? SpReportViewer,
    string? Notas,
    string? ParametrosEjemplo,
    bool Activo);

public record NotaDto(
    int Id,
    string Titulo,
    string Descripcion,
    int Orden,
    bool Activo);

public record DocumentoDto(
    int Id,
    string NombreArchivo,
    string UrlOneDrive,
    string Tipo,
    string? Descripcion,
    int Orden,
    bool Activo);

public record FixDataDto(
    int Id,
    string Nombre,
    string? Descripcion,
    string? Script,
    int Orden,
    bool Activo);
