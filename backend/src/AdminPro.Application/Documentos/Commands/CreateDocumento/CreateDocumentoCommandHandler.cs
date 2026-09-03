using System;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Documentos.Commands.CreateDocumento;

public class CreateDocumentoCommandHandler(AppDbContext dbContext) : IRequestHandler<CreateDocumentoCommand, int>
{
    public async Task<int> Handle(CreateDocumentoCommand request, CancellationToken cancellationToken)
    {
        var applicationExists = await dbContext.Applications.AnyAsync(a => a.Id == request.AplicacionId, cancellationToken);
        if (!applicationExists)
        {
            throw new NotFoundException($"Application {request.AplicacionId} not found.");
        }

        var now = DateTime.UtcNow;
        var documento = new Documento
        {
            AplicacionId = request.AplicacionId,
            NombreArchivo = request.NombreArchivo,
            UrlOneDrive = request.UrlOneDrive,
            Tipo = request.Tipo,
            Descripcion = request.Descripcion,
            Orden = request.Orden,
            Activo = true,
            CreatedAt = now,
            UpdatedAt = now,
        };

        dbContext.Documentos.Add(documento);
        await dbContext.SaveChangesAsync(cancellationToken);

        return documento.Id;
    }
}
