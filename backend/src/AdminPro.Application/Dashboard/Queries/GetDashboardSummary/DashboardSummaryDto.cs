using System.Collections.Generic;

namespace AdminPro.Application.Dashboard.Queries.GetDashboardSummary;

public record DashboardSummaryDto(
    int TotalProjects,
    int TotalApplications,
    int TotalAmbientes,
    int TotalServiciosVinculados,
    IReadOnlyList<int> ApplicationsCreatedLast7Days,
    IReadOnlyList<RecentApplicationDto> RecentApplications,
    ApplicationStatusBreakdownDto StatusBreakdown);

public record RecentApplicationDto(
    int Id,
    string Nombre,
    string ProjectName,
    string? TecnologiaFront,
    string? TecnologiaBack,
    string Status);

public record ApplicationStatusBreakdownDto(int Activo, int EnProgreso, int Pendiente);
