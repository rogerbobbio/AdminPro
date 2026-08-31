using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Application.Projects.Queries.GetProjectById;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AppEntity = AdminPro.Domain.Entities.Application;

namespace AdminPro.Application.Tests.Projects;

public class GetProjectByIdQueryHandlerTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handle_ReturnsDetailWithDatabases()
    {
        using var db = CreateInMemoryContext(nameof(Handle_ReturnsDetailWithDatabases));
        var project = new Project { Nombre = "Acme Corp", Descripcion = "Sistema", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        db.BasesDeDatos.AddRange(
            new BaseDeDatos { ProyectoId = project.Id, Nombre = "SalesDb", Ambiente = "desarrollo", Activo = true },
            new BaseDeDatos { ProyectoId = project.Id, Nombre = "AuthDb", Activo = true });
        await db.SaveChangesAsync();

        var handler = new GetProjectByIdQueryHandler(db);
        var result = await handler.Handle(new GetProjectByIdQuery(project.Id, false), CancellationToken.None);

        result.Nombre.Should().Be("Acme Corp");
        result.BasesDeDatos.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ReturnsRealApplications_NotAlwaysEmpty()
    {
        using var db = CreateInMemoryContext(nameof(Handle_ReturnsRealApplications_NotAlwaysEmpty));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        db.Applications.Add(new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true });
        await db.SaveChangesAsync();

        var handler = new GetProjectByIdQueryHandler(db);
        var result = await handler.Handle(new GetProjectByIdQuery(project.Id, false), CancellationToken.None);

        result.Applications.Should().ContainSingle(a => a.Nombre == "CRM");
    }

    [Fact]
    public async Task Handle_MissingProject_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handle_MissingProject_ThrowsNotFoundException));
        var handler = new GetProjectByIdQueryHandler(db);

        var act = async () => await handler.Handle(new GetProjectByIdQuery(999, false), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
