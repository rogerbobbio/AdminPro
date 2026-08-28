using System;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Behaviors;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using NSubstitute;
using Xunit;
using ValidationException = AdminPro.Application.Common.Exceptions.ValidationException;

namespace AdminPro.Application.Tests.Common.Behaviors;

public class ValidationBehaviorTests
{
    public record FakeRequest(string Name) : IRequest<string>;

    [Fact]
    public async Task Handle_WhenValidationFails_ThrowsValidationExceptionAndDoesNotCallNext()
    {
        var validator = Substitute.For<IValidator<FakeRequest>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<FakeRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { new ValidationFailure("Name", "required") }));

        var next = Substitute.For<RequestHandlerDelegate<string>>();
        var behavior = new ValidationBehavior<FakeRequest, string>([validator]);

        Func<Task> act = () => behavior.Handle(new FakeRequest(""), next, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
        await next.DidNotReceive().Invoke(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenValidationSucceeds_CallsNext()
    {
        var validator = Substitute.For<IValidator<FakeRequest>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<FakeRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var next = Substitute.For<RequestHandlerDelegate<string>>();
        next.Invoke(Arg.Any<CancellationToken>()).Returns("ok");

        var behavior = new ValidationBehavior<FakeRequest, string>([validator]);

        var result = await behavior.Handle(new FakeRequest("valid"), next, CancellationToken.None);

        result.Should().Be("ok");
        await next.Received(1).Invoke(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNoValidatorsRegistered_CallsNext()
    {
        var next = Substitute.For<RequestHandlerDelegate<string>>();
        next.Invoke(Arg.Any<CancellationToken>()).Returns("ok");

        var behavior = new ValidationBehavior<FakeRequest, string>([]);

        var result = await behavior.Handle(new FakeRequest("x"), next, CancellationToken.None);

        result.Should().Be("ok");
    }
}
