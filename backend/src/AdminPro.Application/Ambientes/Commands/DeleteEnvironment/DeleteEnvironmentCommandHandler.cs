using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Ambientes.Commands.DeleteEnvironment;

public class DeleteEnvironmentCommandHandler(AppDbContext dbContext) : IRequestHandler<DeleteEnvironmentCommand>
{
    public async Task Handle(DeleteEnvironmentCommand request, CancellationToken cancellationToken)
    {
        var ambiente = await dbContext.Ambientes.FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Ambiente {request.Id} not found.");

        ambiente.Activo = false;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
