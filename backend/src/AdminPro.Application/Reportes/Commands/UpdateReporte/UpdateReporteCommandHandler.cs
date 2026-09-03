using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Reportes.Commands.UpdateReporte;

public class UpdateReporteCommandHandler(AppDbContext dbContext) : IRequestHandler<UpdateReporteCommand>
{
    public async Task Handle(UpdateReporteCommand request, CancellationToken cancellationToken)
    {
        var reporte = await dbContext.Reportes.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Reporte {request.Id} not found.");

        reporte.ReportCode = request.ReportCode;
        reporte.ReportName = request.ReportName;
        reporte.RegionId = request.RegionId;
        reporte.ReportPath = request.ReportPath;
        reporte.SpTranship = request.SpTranship;
        reporte.SpReportViewer = request.SpReportViewer;
        reporte.Notas = request.Notas;
        reporte.ParametrosEjemplo = request.ParametrosEjemplo;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
