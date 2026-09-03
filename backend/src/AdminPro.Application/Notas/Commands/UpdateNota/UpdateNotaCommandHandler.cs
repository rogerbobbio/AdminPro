using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Notas.Commands.UpdateNota;

public class UpdateNotaCommandHandler(AppDbContext dbContext) : IRequestHandler<UpdateNotaCommand>
{
    public async Task Handle(UpdateNotaCommand request, CancellationToken cancellationToken)
    {
        var nota = await dbContext.Notas.FirstOrDefaultAsync(n => n.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Nota {request.Id} not found.");

        nota.Titulo = request.Titulo;
        nota.Descripcion = request.Descripcion;
        nota.Orden = request.Orden;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
