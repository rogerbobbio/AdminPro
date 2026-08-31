using MediatR;

namespace AdminPro.Application.Applications.Queries.GetApplicationById;

public record GetApplicationByIdQuery(int Id) : IRequest<ApplicationDetailDto>;
