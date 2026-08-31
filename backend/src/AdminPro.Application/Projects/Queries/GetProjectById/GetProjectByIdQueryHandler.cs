using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Projects.Queries.GetProjectById;

public class GetProjectByIdQueryHandler(AppDbContext dbContext) : IRequestHandler<GetProjectByIdQuery, ProjectDetailDto>
{
    public async Task<ProjectDetailDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await dbContext.Projects.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Project {request.Id} not found.");

        var basesQuery = request.IncludeInactiveChildren
            ? dbContext.BasesDeDatos.IgnoreQueryFilters()
            : dbContext.BasesDeDatos;

        var basesDeDatos = await basesQuery
            .Where(d => d.ProyectoId == request.Id)
            .Select(d => new BaseDeDatosDto(d.Id, d.Nombre, d.Servidor, d.DatabaseId, d.LoginName, d.Ambiente, d.Notas, d.Activo))
            .ToListAsync(cancellationToken);

        var applicationsQuery = request.IncludeInactiveChildren
            ? dbContext.Applications.IgnoreQueryFilters()
            : dbContext.Applications;

        var applications = await applicationsQuery
            .Where(a => a.ProyectoId == request.Id)
            .Select(a => new ApplicationSummaryDto(a.Id, a.Nombre, a.TecnologiaFront, a.TecnologiaBack, a.Orden, a.Activo))
            .ToListAsync(cancellationToken);

        return new ProjectDetailDto(
            project.Id,
            project.Nombre,
            project.Descripcion,
            project.Activo,
            project.CreatedAt,
            project.UpdatedAt,
            basesDeDatos,
            applications);
    }
}
