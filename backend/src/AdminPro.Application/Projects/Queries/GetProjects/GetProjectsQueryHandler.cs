using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Projects.Queries.GetProjects;

public class GetProjectsQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetProjectsQuery, IReadOnlyList<ProjectSummaryDto>>
{
    public async Task<IReadOnlyList<ProjectSummaryDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var query = request.IncludeInactive ? dbContext.Projects.IgnoreQueryFilters() : dbContext.Projects;

        return await query
            .OrderBy(p => p.Nombre)
            .Select(p => new ProjectSummaryDto(p.Id, p.Nombre, p.Descripcion, p.Activo, p.CreatedAt, p.UpdatedAt))
            .ToListAsync(cancellationToken);
    }
}
