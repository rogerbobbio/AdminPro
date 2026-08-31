using System;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Projects.Commands.UpdateProject;

public class UpdateProjectCommandHandler(AppDbContext dbContext) : IRequestHandler<UpdateProjectCommand>
{
    public async Task Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await dbContext.Projects.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Project {request.Id} not found.");

        project.Nombre = request.Nombre;
        project.Descripcion = request.Descripcion;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
