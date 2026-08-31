using MediatR;

namespace AdminPro.Application.Projects.Queries.GetProjectById;

public record GetProjectByIdQuery(int Id, bool IncludeInactiveChildren) : IRequest<ProjectDetailDto>;
