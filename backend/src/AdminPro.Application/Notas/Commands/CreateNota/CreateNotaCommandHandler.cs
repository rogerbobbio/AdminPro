using System;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Notas.Commands.CreateNota;

public class CreateNotaCommandHandler(AppDbContext dbContext) : IRequestHandler<CreateNotaCommand, int>
{
    public async Task<int> Handle(CreateNotaCommand request, CancellationToken cancellationToken)
    {
        var applicationExists = await dbContext.Applications.AnyAsync(a => a.Id == request.AplicacionId, cancellationToken);
        if (!applicationExists)
        {
            throw new NotFoundException($"Application {request.AplicacionId} not found.");
        }

        var now = DateTime.UtcNow;
        var nota = new Nota
        {
            AplicacionId = request.AplicacionId,
            Titulo = request.Titulo,
            Descripcion = request.Descripcion,
            Orden = request.Orden,
            Activo = true,
            CreatedAt = now,
            UpdatedAt = now,
        };

        dbContext.Notas.Add(nota);
        await dbContext.SaveChangesAsync(cancellationToken);

        return nota.Id;
    }
}
