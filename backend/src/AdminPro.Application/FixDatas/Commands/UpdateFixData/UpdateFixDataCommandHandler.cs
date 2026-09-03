using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.FixDatas.Commands.UpdateFixData;

public class UpdateFixDataCommandHandler(AppDbContext dbContext) : IRequestHandler<UpdateFixDataCommand>
{
    public async Task Handle(UpdateFixDataCommand request, CancellationToken cancellationToken)
    {
        var fixData = await dbContext.FixDatas.FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"FixData {request.Id} not found.");

        fixData.Nombre = request.Nombre;
        fixData.Descripcion = request.Descripcion;
        fixData.Script = request.Script;
        fixData.Orden = request.Orden;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
