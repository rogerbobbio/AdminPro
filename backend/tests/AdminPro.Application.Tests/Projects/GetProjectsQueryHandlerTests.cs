using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Projects.Queries.GetProjects;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdminPro.Application.Tests.Projects;

public class GetProjectsQueryHandlerTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handle_DefaultExcludesInactive_OrderedByNombre()
    {
        using var db = CreateInMemoryContext(nameof(Handle_DefaultExcludesInactive_OrderedByNombre));
        db.Projects.AddRange(
            new Project { Nombre = "Globex Corp", Activo = true },
            new Project { Nombre = "Acme Corp", Activo = true },
            new Project { Nombre = "Old Co", Activo = false });
        await db.SaveChangesAsync();

        var handler = new GetProjectsQueryHandler(db);
        var result = await handler.Handle(new GetProjectsQuery(false), CancellationToken.None);

        result.Select(p => p.Nombre).Should().Equal("Acme Corp", "Globex Corp");
    }

    [Fact]
    public async Task Handle_IncludeInactiveTrue_ReturnsAll()
    {
        using var db = CreateInMemoryContext(nameof(Handle_IncludeInactiveTrue_ReturnsAll));
        db.Projects.AddRange(
            new Project { Nombre = "Acme Corp", Activo = true },
            new Project { Nombre = "Old Co", Activo = false });
        await db.SaveChangesAsync();

        var handler = new GetProjectsQueryHandler(db);
        var result = await handler.Handle(new GetProjectsQuery(true), CancellationToken.None);

        result.Should().HaveCount(2);
    }
}
