using System;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using MediatR;

namespace AdminPro.Application.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler(AppDbContext dbContext) : IRequestHandler<CreateProjectCommand, int>
{
    public async Task<int> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var project = new Project
        {
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Activo = true,
            CreatedAt = now,
            UpdatedAt = now,
        };

        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync(cancellationToken);

        return project.Id;
    }
}
