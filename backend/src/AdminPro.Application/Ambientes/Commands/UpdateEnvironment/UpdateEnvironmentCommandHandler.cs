using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Ambientes.Commands.UpdateEnvironment;

public class UpdateEnvironmentCommandHandler(AppDbContext dbContext) : IRequestHandler<UpdateEnvironmentCommand>
{
    public async Task Handle(UpdateEnvironmentCommand request, CancellationToken cancellationToken)
    {
        var ambiente = await dbContext.Ambientes.FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Ambiente {request.Id} not found.");

        ambiente.Nombre = request.Nombre;
        ambiente.Url = request.Url;
        ambiente.EsWebApi = request.EsWebApi;
        ambiente.Notas = request.Notas;
        ambiente.Orden = request.Orden;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
