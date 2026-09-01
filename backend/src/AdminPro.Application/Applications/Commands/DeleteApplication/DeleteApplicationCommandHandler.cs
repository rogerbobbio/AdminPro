using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Applications.Commands.DeleteApplication;

public class DeleteApplicationCommandHandler(AppDbContext dbContext) : IRequestHandler<DeleteApplicationCommand>
{
    public async Task Handle(DeleteApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = await dbContext.Applications.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Application {request.Id} not found.");

        application.Activo = false;

        var ambientes = await dbContext.Ambientes
            .Where(e => e.AplicacionId == request.Id)
            .ToListAsync(cancellationToken);
        foreach (var ambiente in ambientes)
        {
            ambiente.Activo = false;
        }

        // Reportes, Notas, Documentos, FixDatas, AplicacionServicios cascade here too (rule APP-002),
        // once those entities get their own CRUD in the `application-children` change (Phase 5).

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
