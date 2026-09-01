using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Applications.Queries.GetApplicationsByProject;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AppEntity = AdminPro.Domain.Entities.Application;

namespace AdminPro.Application.Tests.Applications;

public class GetApplicationsByProjectQueryHandlerTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"{nameof(GetApplicationsByProjectQueryHandlerTests)}.{dbName}")
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handle_DefaultExcludesInactive_OrderedByOrdenThenNombre()
    {
        using var db = CreateInMemoryContext(nameof(Handle_DefaultExcludesInactive_OrderedByOrdenThenNombre));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        db.Applications.AddRange(
            new AppEntity { ProyectoId = project.Id, Nombre = "Billing", Orden = 1, Activo = true },
            new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Orden = 0, Activo = true },
            new AppEntity { ProyectoId = project.Id, Nombre = "Old App", Orden = 0, Activo = false });
        await db.SaveChangesAsync();

        var handler = new GetApplicationsByProjectQueryHandler(db);
        var result = await handler.Handle(new GetApplicationsByProjectQuery(project.Id, false), CancellationToken.None);

        result.Select(a => a.Nombre).Should().Equal("CRM", "Billing");
    }

    [Fact]
    public async Task Handle_IncludeInactiveTrue_ReturnsAll()
    {
        using var db = CreateInMemoryContext(nameof(Handle_IncludeInactiveTrue_ReturnsAll));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        db.Applications.AddRange(
            new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true },
            new AppEntity { ProyectoId = project.Id, Nombre = "Old App", Activo = false });
        await db.SaveChangesAsync();

        var handler = new GetApplicationsByProjectQueryHandler(db);
        var result = await handler.Handle(new GetApplicationsByProjectQuery(project.Id, true), CancellationToken.None);

        result.Should().HaveCount(2);
    }
}
