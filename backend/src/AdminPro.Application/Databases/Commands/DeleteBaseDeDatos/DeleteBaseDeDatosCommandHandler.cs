using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Databases.Commands.DeleteBaseDeDatos;

public class DeleteBaseDeDatosCommandHandler(AppDbContext dbContext) : IRequestHandler<DeleteBaseDeDatosCommand>
{
    public async Task Handle(DeleteBaseDeDatosCommand request, CancellationToken cancellationToken)
    {
        var database = await dbContext.BasesDeDatos.FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"BaseDeDatos {request.Id} not found.");

        database.Activo = false;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
