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
        var totalApplications = await dbContext.Applications.CountAsync(cancellationToken);

        var recentApplications = await dbContext.Applications
            .Include(a => a.Project)
            .OrderByDescending(a => a.UpdatedAt)
            .Take(RecentApplicationsCount)
            .Select(a => new RecentApplicationDto(
                a.Id,
                a.Nombre,
                a.Project.Nombre,
                a.TecnologiaFront,
                a.TecnologiaBack,
                a.UpdatedAt))
            .ToListAsync(cancellationToken);

        return new DashboardSummaryDto(totalProjects, totalApplications, recentApplications);
    }
}
