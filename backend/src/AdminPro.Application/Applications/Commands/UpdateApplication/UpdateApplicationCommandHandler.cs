using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Applications.Commands.UpdateApplication;

public class UpdateApplicationCommandHandler(AppDbContext dbContext) : IRequestHandler<UpdateApplicationCommand>
{
    public async Task Handle(UpdateApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = await dbContext.Applications.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Application {request.Id} not found.");

        application.Nombre = request.Nombre;
        application.Descripcion = request.Descripcion;
        application.Tipo = request.Tipo;
        application.TecnologiaFront = request.TecnologiaFront;
        application.TecnologiaBack = request.TecnologiaBack;
        application.RamaDesarrollo = request.RamaDesarrollo;
        application.ApplicationName = request.ApplicationName;
        application.RutaLocal = request.RutaLocal;
        application.RutaGit = request.RutaGit;
        application.ComoSeLevanta = request.ComoSeLevanta;
        application.NotasCompilacion = request.NotasCompilacion;
        application.Orden = request.Orden;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
