using System;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AppEntity = AdminPro.Domain.Entities.Application;

namespace AdminPro.Application.Applications.Commands.CreateApplication;

public class CreateApplicationCommandHandler(AppDbContext dbContext) : IRequestHandler<CreateApplicationCommand, int>
{
    public async Task<int> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        var projectExists = await dbContext.Projects.AnyAsync(p => p.Id == request.ProyectoId, cancellationToken);
        if (!projectExists)
        {
            throw new NotFoundException($"Project {request.ProyectoId} not found.");
        }

        var now = DateTime.UtcNow;
        var application = new AppEntity
        {
            ProyectoId = request.ProyectoId,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Tipo = request.Tipo,
            TecnologiaFront = request.TecnologiaFront,
            TecnologiaBack = request.TecnologiaBack,
            RamaDesarrollo = request.RamaDesarrollo,
            ApplicationName = request.ApplicationName,
            RutaLocal = request.RutaLocal,
            RutaGit = request.RutaGit,
            ComoSeLevanta = request.ComoSeLevanta,
            NotasCompilacion = request.NotasCompilacion,
            Orden = request.Orden,
            Activo = true,
            CreatedAt = now,
            UpdatedAt = now,
        };

        dbContext.Applications.Add(application);
        await dbContext.SaveChangesAsync(cancellationToken);

        return application.Id;
    }
}
