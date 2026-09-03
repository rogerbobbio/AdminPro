using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Notas.Commands.DeleteNota;

public class DeleteNotaCommandHandler(AppDbContext dbContext) : IRequestHandler<DeleteNotaCommand>
{
    public async Task Handle(DeleteNotaCommand request, CancellationToken cancellationToken)
    {
        var nota = await dbContext.Notas.FirstOrDefaultAsync(n => n.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Nota {request.Id} not found.");

        nota.Activo = false;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
