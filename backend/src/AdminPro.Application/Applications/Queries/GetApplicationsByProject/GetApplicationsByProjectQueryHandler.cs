using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Applications.Queries.GetApplicationsByProject;

public class GetApplicationsByProjectQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetApplicationsByProjectQuery, IReadOnlyList<ApplicationSummaryDto>>
{
    public async Task<IReadOnlyList<ApplicationSummaryDto>> Handle(
        GetApplicationsByProjectQuery request, CancellationToken cancellationToken)
    {
        var query = request.IncludeInactive ? dbContext.Applications.IgnoreQueryFilters() : dbContext.Applications;

        return await query
            .Where(a => a.ProyectoId == request.ProyectoId)
            .OrderBy(a => a.Orden).ThenBy(a => a.Nombre)
            .Select(a => new ApplicationSummaryDto(a.Id, a.Nombre, a.TecnologiaFront, a.TecnologiaBack, a.Orden, a.Activo))
            .ToListAsync(cancellationToken);
    }
}
