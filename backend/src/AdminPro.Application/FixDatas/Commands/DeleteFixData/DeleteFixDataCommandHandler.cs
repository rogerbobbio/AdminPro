using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.FixDatas.Commands.DeleteFixData;

public class DeleteFixDataCommandHandler(AppDbContext dbContext) : IRequestHandler<DeleteFixDataCommand>
{
    public async Task Handle(DeleteFixDataCommand request, CancellationToken cancellationToken)
    {
        var fixData = await dbContext.FixDatas.FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"FixData {request.Id} not found.");

        fixData.Activo = false;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
