using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Modulos.Queries.GetModulos;

public class GetModulosQueryHandler(AppDbContext dbContext) : IRequestHandler<GetModulosQuery, IReadOnlyList<ModuloDto>>
{
    public async Task<IReadOnlyList<ModuloDto>> Handle(GetModulosQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.Modulos
            .OrderBy(m => m.Orden)
            .Select(m => new ModuloDto(m.Id, m.Nombre, m.Icono, m.RutaBase, m.Color, m.Orden))
            .ToListAsync(cancellationToken);
    }
}
