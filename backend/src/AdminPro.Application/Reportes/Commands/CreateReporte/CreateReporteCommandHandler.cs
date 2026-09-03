using System;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Reportes.Commands.CreateReporte;

public class CreateReporteCommandHandler(AppDbContext dbContext) : IRequestHandler<CreateReporteCommand, int>
{
    public async Task<int> Handle(CreateReporteCommand request, CancellationToken cancellationToken)
    {
        var applicationExists = await dbContext.Applications.AnyAsync(a => a.Id == request.AplicacionId, cancellationToken);
        if (!applicationExists)
        {
            throw new NotFoundException($"Application {request.AplicacionId} not found.");
        }

        var now = DateTime.UtcNow;
        var reporte = new Reporte
        {
            AplicacionId = request.AplicacionId,
            ReportCode = request.ReportCode,
            ReportName = request.ReportName,
            RegionId = request.RegionId,
            ReportPath = request.ReportPath,
            SpTranship = request.SpTranship,
            SpReportViewer = request.SpReportViewer,
            Notas = request.Notas,
            ParametrosEjemplo = request.ParametrosEjemplo,
            Activo = true,
            CreatedAt = now,
            UpdatedAt = now,
        };

        dbContext.Reportes.Add(reporte);
        await dbContext.SaveChangesAsync(cancellationToken);

        return reporte.Id;
    }
}
