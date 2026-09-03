using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Applications.Queries.GetApplicationById;

public class GetApplicationByIdQueryHandler(AppDbContext dbContext) : IRequestHandler<GetApplicationByIdQuery, ApplicationDetailDto>
{
    public async Task<ApplicationDetailDto> Handle(GetApplicationByIdQuery request, CancellationToken cancellationToken)
    {
        var application = await dbContext.Applications.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Application {request.Id} not found.");

        var ambientes = await dbContext.Ambientes
            .Where(e => e.AplicacionId == request.Id)
            .OrderBy(e => e.Orden)
            .Select(e => new AmbienteDto(e.Id, e.Nombre, e.Url, e.EsWebApi, e.Notas, e.Orden, e.Activo))
            .ToListAsync(cancellationToken);

        var reportes = await dbContext.Reportes
            .Where(r => r.AplicacionId == request.Id)
            .OrderBy(r => r.ReportCode)
            .Select(r => new ReporteDto(r.Id, r.ReportCode, r.ReportName, r.RegionId, r.ReportPath, r.SpTranship, r.SpReportViewer, r.Notas, r.ParametrosEjemplo, r.Activo))
            .ToListAsync(cancellationToken);

        var notas = await dbContext.Notas
            .Where(n => n.AplicacionId == request.Id)
            .OrderBy(n => n.Orden)
            .Select(n => new NotaDto(n.Id, n.Titulo, n.Descripcion, n.Orden, n.Activo))
            .ToListAsync(cancellationToken);

        var documentos = await dbContext.Documentos
            .Where(d => d.AplicacionId == request.Id)
            .OrderBy(d => d.Orden)
            .Select(d => new DocumentoDto(d.Id, d.NombreArchivo, d.UrlOneDrive, d.Tipo, d.Descripcion, d.Orden, d.Activo))
            .ToListAsync(cancellationToken);

        var fixDatas = await dbContext.FixDatas
            .Where(f => f.AplicacionId == request.Id)
            .OrderBy(f => f.Orden)
            .Select(f => new FixDataDto(f.Id, f.Nombre, f.Descripcion, f.Script, f.Orden, f.Activo))
            .ToListAsync(cancellationToken);

        return new ApplicationDetailDto(
            application.Id,
            application.ProyectoId,
            application.Nombre,
            application.Descripcion,
            application.Tipo,
            application.TecnologiaFront,
            application.TecnologiaBack,
            application.RamaDesarrollo,
            application.ApplicationName,
            application.RutaLocal,
            application.RutaGit,
            application.ComoSeLevanta,
            application.NotasCompilacion,
            application.Orden,
            application.Activo,
            application.CreatedAt,
            application.UpdatedAt,
            ambientes,
            reportes,
            notas,
            documentos,
            fixDatas,
            []);
    }
}
