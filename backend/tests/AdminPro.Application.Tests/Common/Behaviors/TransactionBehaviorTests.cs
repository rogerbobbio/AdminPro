using System;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common;
using AdminPro.Application.Common.Behaviors;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace AdminPro.Application.Tests.Common.Behaviors;

public class TransactionBehaviorTests
{
    public record FakeCommand(string Name) : ICommand<string>;
    public record FakeQuery(string Name) : IRequest<string>;

    private static AppDbContext CreateSqliteContext(SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handle_CommandRequest_OpensAndCommitsTransactionOnSuccess()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        await using var db = CreateSqliteContext(connection);
        await db.Database.EnsureCreatedAsync();

        var next = Substitute.For<RequestHandlerDelegate<string>>();
        next.Invoke(Arg.Any<CancellationToken>()).Returns(_ =>
        {
            db.Database.CurrentTransaction.Should().NotBeNull();
            return Task.FromResult("ok");
        });

        var behavior = new TransactionBehavior<FakeCommand, string>(db);

        var result = await behavior.Handle(new FakeCommand("x"), next, CancellationToken.None);

        result.Should().Be("ok");
        db.Database.CurrentTransaction.Should().BeNull();
    }

    [Fact]
    public async Task Handle_CommandRequest_RollsBackOnException()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        await using var db = CreateSqliteContext(connection);
        await db.Database.EnsureCreatedAsync();

        RequestHandlerDelegate<string> next = _ => throw new InvalidOperationException("boom");

        var behavior = new TransactionBehavior<FakeCommand, string>(db);

        Func<Task> act = () => behavior.Handle(new FakeCommand("x"), next, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
        db.Database.CurrentTransaction.Should().BeNull();
    }

    [Fact]
    public async Task Handle_NonCommandRequest_DoesNotOpenTransaction()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        await using var db = CreateSqliteContext(connection);
        await db.Database.EnsureCreatedAsync();

        var next = Substitute.For<RequestHandlerDelegate<string>>();
        next.Invoke(Arg.Any<CancellationToken>()).Returns(_ =>
        {
            db.Database.CurrentTransaction.Should().BeNull();
            return Task.FromResult("ok");
        });

        var behavior = new TransactionBehavior<FakeQuery, string>(db);

        var result = await behavior.Handle(new FakeQuery("x"), next, CancellationToken.None);

        result.Should().Be("ok");
    }
}
