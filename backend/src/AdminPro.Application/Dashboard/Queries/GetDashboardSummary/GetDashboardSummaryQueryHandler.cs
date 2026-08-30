using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Dashboard.Queries.GetDashboardSummary;

public class GetDashboardSummaryQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private const int RecentApplicationsCount = 5;

    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var totalProjects = await dbContext.Projects.CountAsync(cancellationToken);
        var totalAmbientes = await dbContext.Ambientes.CountAsync(cancellationToken);
        var totalServiciosVinculados = await dbContext.AplicacionServicios
            .Select(a => a.ServicioId)
            .Distinct()
            .CountAsync(cancellationToken);

        var applications = await dbContext.Applications
            .Include(a => a.Project)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

        var weeklySeries = BuildLast7DaysSeries(applications.Select(a => a.CreatedAt));

        var recentApplications = applications
            .Take(RecentApplicationsCount)
            .Select(a => new RecentApplicationDto(
                a.Id,
                a.Nombre,
                a.Project.Nombre,
                a.TecnologiaFront,
                a.TecnologiaBack,
                "Activo"))
            .ToList();

        var statusBreakdown = new ApplicationStatusBreakdownDto(applications.Count, 0, 0);

        return new DashboardSummaryDto(
            totalProjects,
            applications.Count,
            totalAmbientes,
            totalServiciosVinculados,
            weeklySeries,
            recentApplications,
            statusBreakdown);
    }

    private static IReadOnlyList<int> BuildLast7DaysSeries(IEnumerable<DateTime> createdAtTimestamps)
    {
        var today = DateTime.UtcNow.Date;
        var countsByDay = createdAtTimestamps
            .GroupBy(c => c.Date)
            .ToDictionary(g => g.Key, g => g.Count());

        return Enumerable.Range(0, 7)
            .Select(offset => today.AddDays(-6 + offset))
            .Select(day => countsByDay.GetValueOrDefault(day, 0))
            .ToList();
    }
}
