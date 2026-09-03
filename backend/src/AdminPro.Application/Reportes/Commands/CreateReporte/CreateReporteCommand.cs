using AdminPro.Application.Common;

namespace AdminPro.Application.Reportes.Commands.CreateReporte;

public record CreateReporteCommand(
    int AplicacionId,
    string ReportCode,
    string ReportName,
    string? RegionId,
    string? ReportPath,
    string? SpTranship,
    string? SpReportViewer,
    string? Notas,
    string? ParametrosEjemplo) : ICommand<int>;
