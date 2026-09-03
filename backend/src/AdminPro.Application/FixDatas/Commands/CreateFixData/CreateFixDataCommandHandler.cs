using System;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FixDataEntity = AdminPro.Domain.Entities.FixData;

namespace AdminPro.Application.FixDatas.Commands.CreateFixData;

public class CreateFixDataCommandHandler(AppDbContext dbContext) : IRequestHandler<CreateFixDataCommand, int>
{
    public async Task<int> Handle(CreateFixDataCommand request, CancellationToken cancellationToken)
    {
        var applicationExists = await dbContext.Applications.AnyAsync(a => a.Id == request.AplicacionId, cancellationToken);
        if (!applicationExists)
        {
            throw new NotFoundException($"Application {request.AplicacionId} not found.");
        }

        var now = DateTime.UtcNow;
        var fixData = new FixDataEntity
        {
            AplicacionId = request.AplicacionId,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Script = request.Script,
            Orden = request.Orden,
            Activo = true,
            CreatedAt = now,
            UpdatedAt = now,
        };

        dbContext.FixDatas.Add(fixData);
        await dbContext.SaveChangesAsync(cancellationToken);

        return fixData.Id;
    }
}
