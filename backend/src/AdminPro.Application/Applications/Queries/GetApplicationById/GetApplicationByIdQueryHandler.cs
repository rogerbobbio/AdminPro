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
            [],
            [],
            [],
            [],
            []);
    }
}
