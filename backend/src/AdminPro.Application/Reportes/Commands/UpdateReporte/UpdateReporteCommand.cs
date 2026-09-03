using AdminPro.Application.Common;

namespace AdminPro.Application.Reportes.Commands.UpdateReporte;

public record UpdateReporteCommand(
    int Id,
    string ReportCode,
    string ReportName,
    string? RegionId,
    string? ReportPath,
    string? SpTranship,
    string? SpReportViewer,
    string? Notas,
    string? ParametrosEjemplo) : ICommand;
