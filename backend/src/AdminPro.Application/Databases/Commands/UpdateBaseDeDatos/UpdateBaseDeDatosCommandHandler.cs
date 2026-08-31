using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Databases.Commands.UpdateBaseDeDatos;

public class UpdateBaseDeDatosCommandHandler(AppDbContext dbContext) : IRequestHandler<UpdateBaseDeDatosCommand>
{
    public async Task Handle(UpdateBaseDeDatosCommand request, CancellationToken cancellationToken)
    {
        var database = await dbContext.BasesDeDatos.FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"BaseDeDatos {request.Id} not found.");

        database.Nombre = request.Nombre;
        database.Servidor = request.Servidor;
        database.DatabaseId = request.DatabaseId;
        database.LoginName = request.LoginName;
        database.Password = request.Password;
        database.Ambiente = request.Ambiente;
        database.Notas = request.Notas;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
