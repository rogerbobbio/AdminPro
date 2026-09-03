using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Reportes.Commands.DeleteReporte;

public class DeleteReporteCommandHandler(AppDbContext dbContext) : IRequestHandler<DeleteReporteCommand>
{
    public async Task Handle(DeleteReporteCommand request, CancellationToken cancellationToken)
    {
        var reporte = await dbContext.Reportes.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Reporte {request.Id} not found.");

        reporte.Activo = false;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
