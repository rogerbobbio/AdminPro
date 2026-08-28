using AdminPro.Application.Common;
using FluentAssertions;
using MediatR;
using Xunit;

namespace AdminPro.Application.Tests.Common;

public class ICommandTests
{
    private sealed class FakeCommand : ICommand<int>
    {
    }

    private sealed class FakeVoidCommand : ICommand
    {
    }

    [Fact]
    public void CommandWithResponse_IsAMediatRRequest()
    {
        new FakeCommand().Should().BeAssignableTo<IRequest<int>>();
    }

    [Fact]
    public void VoidCommand_IsAMediatRRequest()
    {
        new FakeVoidCommand().Should().BeAssignableTo<IRequest>();
    }
}
