using System.Collections.Generic;
using MediatR;

namespace AdminPro.Application.Applications.Queries.GetApplicationsByProject;

public record GetApplicationsByProjectQuery(int ProyectoId, bool IncludeInactive) : IRequest<IReadOnlyList<ApplicationSummaryDto>>;
