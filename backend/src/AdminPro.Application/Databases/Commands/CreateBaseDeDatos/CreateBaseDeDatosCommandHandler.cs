using System;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DatabaseEntity = AdminPro.Domain.Entities.BaseDeDatos;

namespace AdminPro.Application.Databases.Commands.CreateBaseDeDatos;

public class CreateBaseDeDatosCommandHandler(AppDbContext dbContext) : IRequestHandler<CreateBaseDeDatosCommand, int>
{
    public async Task<int> Handle(CreateBaseDeDatosCommand request, CancellationToken cancellationToken)
    {
        var projectExists = await dbContext.Projects.AnyAsync(p => p.Id == request.ProyectoId, cancellationToken);
        if (!projectExists)
        {
            throw new NotFoundException($"Project {request.ProyectoId} not found.");
        }

        var now = DateTime.UtcNow;
        var baseDeDatos = new DatabaseEntity
        {
            ProyectoId = request.ProyectoId,
            Nombre = request.Nombre,
            Servidor = request.Servidor,
            DatabaseId = request.DatabaseId,
            LoginName = request.LoginName,
            Ambiente = request.Ambiente,
            Notas = request.Notas,
            Activo = true,
            CreatedAt = now,
            UpdatedAt = now,
        };

        dbContext.BasesDeDatos.Add(baseDeDatos);
        await dbContext.SaveChangesAsync(cancellationToken);

        return baseDeDatos.Id;
    }
}
