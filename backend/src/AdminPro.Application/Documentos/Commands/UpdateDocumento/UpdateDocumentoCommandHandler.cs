using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Documentos.Commands.UpdateDocumento;

public class UpdateDocumentoCommandHandler(AppDbContext dbContext) : IRequestHandler<UpdateDocumentoCommand>
{
    public async Task Handle(UpdateDocumentoCommand request, CancellationToken cancellationToken)
    {
        var documento = await dbContext.Documentos.FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Documento {request.Id} not found.");

        documento.NombreArchivo = request.NombreArchivo;
        documento.UrlOneDrive = request.UrlOneDrive;
        documento.Tipo = request.Tipo;
        documento.Descripcion = request.Descripcion;
        documento.Orden = request.Orden;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
