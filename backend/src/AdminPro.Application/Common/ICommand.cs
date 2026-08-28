using MediatR;

namespace AdminPro.Application.Common;

/// <summary>
/// Non-MediatR marker used only so TransactionBehavior can detect commands regardless of
/// their response type. ICommand&lt;TResponse&gt; deliberately does NOT inherit ICommand
/// (which is IRequest, i.e. IRequest&lt;Unit&gt;) - a type implementing both IRequest and
/// IRequest&lt;TResponse&gt; makes ISender.Send(...) ambiguous at every call site.
/// </summary>
public interface ICommandMarker
{
}

public interface ICommand : ICommandMarker, IRequest
{
}

public interface ICommand<out TResponse> : ICommandMarker, IRequest<TResponse>
{
}
