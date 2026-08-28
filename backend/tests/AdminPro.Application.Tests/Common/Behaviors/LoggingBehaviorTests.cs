using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Behaviors;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace AdminPro.Application.Tests.Common.Behaviors;

public class LoggingBehaviorTests
{
    public record FakeRequest(string Name) : IRequest<string>;

    private sealed class FakeLogger<T> : ILogger<T>
    {
        public System.Collections.Generic.List<(LogLevel Level, string Message)> Entries { get; } = [];

        public System.IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            System.Exception? exception,
            System.Func<TState, System.Exception?, string> formatter)
        {
            Entries.Add((logLevel, formatter(state, exception)));
        }
    }

    [Fact]
    public async Task Handle_LogsBeforeAndAfterNext()
    {
        var logger = new FakeLogger<LoggingBehavior<FakeRequest, string>>();
        var next = Substitute.For<RequestHandlerDelegate<string>>();
        next.Invoke(Arg.Any<CancellationToken>()).Returns("ok");

        var behavior = new LoggingBehavior<FakeRequest, string>(logger);

        var result = await behavior.Handle(new FakeRequest("x"), next, CancellationToken.None);

        result.Should().Be("ok");
        logger.Entries.Should().HaveCount(2);
        logger.Entries.Should().OnlyContain(e => e.Level == LogLevel.Information);
        logger.Entries[0].Message.Should().Contain(nameof(FakeRequest));
        logger.Entries[1].Message.Should().Contain(nameof(FakeRequest));
    }
}
