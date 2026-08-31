using System.Collections.Generic;
using MediatR;

namespace AdminPro.Application.Projects.Queries.GetProjects;

public record GetProjectsQuery(bool IncludeInactive) : IRequest<IReadOnlyList<ProjectSummaryDto>>;
