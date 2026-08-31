using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Projects.Commands.DeleteProject;

public class DeleteProjectCommandHandler(AppDbContext dbContext) : IRequestHandler<DeleteProjectCommand>
{
    public async Task Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await dbContext.Projects.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Project {request.Id} not found.");

        project.Activo = false;

        var basesDeDatos = await dbContext.BasesDeDatos
            .Where(d => d.ProyectoId == request.Id)
            .ToListAsync(cancellationToken);
        foreach (var baseDeDatos in basesDeDatos)
        {
            baseDeDatos.Activo = false;
        }

        var applications = await dbContext.Applications
            .Where(a => a.ProyectoId == request.Id)
            .ToListAsync(cancellationToken);
        foreach (var application in applications)
        {
            application.Activo = false;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
