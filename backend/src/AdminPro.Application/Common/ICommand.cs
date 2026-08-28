using MediatR;

namespace AdminPro.Application.Common;

public interface ICommand : IRequest
{
}

public interface ICommand<out TResponse> : ICommand, IRequest<TResponse>
{
}
