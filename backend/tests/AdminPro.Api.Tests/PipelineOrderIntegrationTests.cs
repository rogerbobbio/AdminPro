using AdminPro.Api.Tests.TestSupport;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AdminPro.Api.Tests;

public class PipelineOrderIntegrationTests : IClassFixture<ContainerizedApiFactory>
{
    private readonly ContainerizedApiFactory _factory;

    public PipelineOrderIntegrationTests(ContainerizedApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ValidCommand_RunsThroughFullPipelineAndCommits()
    {
        using var scope = _factory.Services.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var id = await sender.Send(new RecordNoteCommand("Acme Corp"));

        id.Should().BeGreaterThan(0);

        var persisted = await db.Projects.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == id);
        persisted.Should().NotBeNull();
        persisted!.Nombre.Should().Be("Acme Corp");

        var captured = string.Join(" | ", _factory.LogCapture.Messages);
        _factory.LogCapture.Messages.Should().Contain(m => m.Contains(nameof(RecordNoteCommand)) && m.Contains("Handling"), captured);
        _factory.LogCapture.Messages.Should().Contain(m => m.Contains(nameof(RecordNoteCommand)) && m.Contains("Handled"), captured);
    }

    [Fact]
    public async Task InvalidCommand_FailsValidationAndNeverCommits()
    {
        using var scope = _factory.Services.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var countBefore = await db.Projects.IgnoreQueryFilters().CountAsync();

        Func<Task> act = () => sender.Send(new RecordNoteCommand(""));

        await act.Should().ThrowAsync<ValidationException>();

        var countAfter = await db.Projects.IgnoreQueryFilters().CountAsync();
        countAfter.Should().Be(countBefore);
    }
}
