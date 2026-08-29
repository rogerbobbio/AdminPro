using System.Collections.Generic;
using MediatR;

namespace AdminPro.Application.Modulos.Queries.GetModulos;

public record GetModulosQuery : IRequest<IReadOnlyList<ModuloDto>>;
