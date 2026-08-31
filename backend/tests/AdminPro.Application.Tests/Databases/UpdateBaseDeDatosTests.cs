using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Application.Databases.Commands.UpdateBaseDeDatos;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using DatabaseEntity = AdminPro.Domain.Entities.BaseDeDatos;

namespace AdminPro.Application.Tests.Databases;

public class UpdateBaseDeDatosTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handler_UpdatesFields()
    {
        using var db = CreateInMemoryContext(nameof(Handler_UpdatesFields));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        var database = new DatabaseEntity { ProyectoId = project.Id, Nombre = "SalesDb", Ambiente = "desarrollo", Activo = true };
        db.BasesDeDatos.Add(database);
        await db.SaveChangesAsync();

        var handler = new UpdateBaseDeDatosCommandHandler(db);
        await handler.Handle(
            new UpdateBaseDeDatosCommand(database.Id, "SalesDb", null, null, null, null, "uat", null),
            CancellationToken.None);

        var updated = await db.BasesDeDatos.FindAsync(database.Id);
        updated!.Ambiente.Should().Be("uat");
    }

    [Fact]
    public async Task Handler_UpdatesConnectionCredentials()
    {
        using var db = CreateInMemoryContext(nameof(Handler_UpdatesConnectionCredentials));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        var database = new DatabaseEntity { ProyectoId = project.Id, Nombre = "SalesDb", Activo = true };
        db.BasesDeDatos.Add(database);
        await db.SaveChangesAsync();

        var handler = new UpdateBaseDeDatosCommandHandler(db);
        await handler.Handle(
            new UpdateBaseDeDatosCommand(database.Id, "SalesDb", null, 42, "app_user", "new-secret", null, null),
            CancellationToken.None);

        var updated = await db.BasesDeDatos.FindAsync(database.Id);
        updated!.DatabaseId.Should().Be(42);
        updated.LoginName.Should().Be("app_user");
        updated.Password.Should().Be("new-secret");
    }

    [Fact]
    public async Task Handler_MissingDatabase_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handler_MissingDatabase_ThrowsNotFoundException));
        var handler = new UpdateBaseDeDatosCommandHandler(db);

        var act = async () => await handler.Handle(
            new UpdateBaseDeDatosCommand(999, "SalesDb", null, null, null, null, null, null),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
