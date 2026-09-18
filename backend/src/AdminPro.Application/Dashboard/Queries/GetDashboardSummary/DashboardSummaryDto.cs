using System;
using System.Collections.Generic;

namespace AdminPro.Application.Dashboard.Queries.GetDashboardSummary;

public record DashboardSummaryDto(
    int TotalProjects,
    int TotalApplications,
    IReadOnlyList<RecentApplicationDto> RecentApplications);

public record RecentApplicationDto(
    int Id,
    string Nombre,
    string ProjectName,
    string? TecnologiaFront,
    string? TecnologiaBack,
    DateTime UpdatedAt);
