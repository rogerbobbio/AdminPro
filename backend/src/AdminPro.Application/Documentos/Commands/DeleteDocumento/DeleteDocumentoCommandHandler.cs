using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Documentos.Commands.DeleteDocumento;

public class DeleteDocumentoCommandHandler(AppDbContext dbContext) : IRequestHandler<DeleteDocumentoCommand>
{
    public async Task Handle(DeleteDocumentoCommand request, CancellationToken cancellationToken)
    {
        var documento = await dbContext.Documentos.FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Documento {request.Id} not found.");

        documento.Activo = false;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
