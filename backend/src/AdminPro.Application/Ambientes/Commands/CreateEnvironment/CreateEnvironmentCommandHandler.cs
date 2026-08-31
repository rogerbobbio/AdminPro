using System;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Ambientes.Commands.CreateEnvironment;

public class CreateEnvironmentCommandHandler(AppDbContext dbContext) : IRequestHandler<CreateEnvironmentCommand, int>
{
    public async Task<int> Handle(CreateEnvironmentCommand request, CancellationToken cancellationToken)
    {
        var applicationExists = await dbContext.Applications.AnyAsync(a => a.Id == request.AplicacionId, cancellationToken);
        if (!applicationExists)
        {
            throw new NotFoundException($"Application {request.AplicacionId} not found.");
        }

        var now = DateTime.UtcNow;
        var ambiente = new Ambiente
        {
            AplicacionId = request.AplicacionId,
            Nombre = request.Nombre,
            Url = request.Url,
            EsWebApi = request.EsWebApi,
            Notas = request.Notas,
            Orden = request.Orden,
            Activo = true,
            CreatedAt = now,
            UpdatedAt = now,
        };

        dbContext.Ambientes.Add(ambiente);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ambiente.Id;
    }
}
