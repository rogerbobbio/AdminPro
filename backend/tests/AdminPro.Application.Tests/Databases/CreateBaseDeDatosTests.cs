using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Application.Databases.Commands.CreateBaseDeDatos;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdminPro.Application.Tests.Databases;

public class CreateBaseDeDatosTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handler_CreatesDatabaseScopedToProject()
    {
        using var db = CreateInMemoryContext(nameof(Handler_CreatesDatabaseScopedToProject));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        var handler = new CreateBaseDeDatosCommandHandler(db);
        var id = await handler.Handle(
            new CreateBaseDeDatosCommand(project.Id, "SalesDb", null, null, null, null, "desarrollo", null),
            CancellationToken.None);

        var created = await db.BasesDeDatos.FindAsync(id);
        created!.Nombre.Should().Be("SalesDb");
        created.ProyectoId.Should().Be(project.Id);
    }

    [Fact]
    public async Task Handler_CreatesDatabaseWithConnectionCredentials()
    {
        using var db = CreateInMemoryContext(nameof(Handler_CreatesDatabaseWithConnectionCredentials));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        var handler = new CreateBaseDeDatosCommandHandler(db);
        var id = await handler.Handle(
            new CreateBaseDeDatosCommand(project.Id, "SalesDb", null, 42, "app_user", "s3cr3t", null, null),
            CancellationToken.None);

        var created = await db.BasesDeDatos.FindAsync(id);
        created!.DatabaseId.Should().Be(42);
        created.LoginName.Should().Be("app_user");
        created.Password.Should().Be("s3cr3t");
    }

    [Fact]
    public async Task Handler_MissingProject_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handler_MissingProject_ThrowsNotFoundException));
        var handler = new CreateBaseDeDatosCommandHandler(db);

        var act = async () => await handler.Handle(
            new CreateBaseDeDatosCommand(999, "SalesDb", null, null, null, null, null, null),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
