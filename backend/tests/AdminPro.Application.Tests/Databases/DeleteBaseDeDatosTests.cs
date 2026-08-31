using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Application.Databases.Commands.DeleteBaseDeDatos;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using DatabaseEntity = AdminPro.Domain.Entities.BaseDeDatos;

namespace AdminPro.Application.Tests.Databases;

public class DeleteBaseDeDatosTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handler_SetsActivoFalse()
    {
        using var db = CreateInMemoryContext(nameof(Handler_SetsActivoFalse));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        var database = new DatabaseEntity { ProyectoId = project.Id, Nombre = "SalesDb", Activo = true };
        db.BasesDeDatos.Add(database);
        await db.SaveChangesAsync();

        var handler = new DeleteBaseDeDatosCommandHandler(db);
        await handler.Handle(new DeleteBaseDeDatosCommand(database.Id), CancellationToken.None);

        (await db.BasesDeDatos.IgnoreQueryFilters().FirstAsync(d => d.Id == database.Id)).Activo.Should().BeFalse();
    }

    [Fact]
    public async Task Handler_MissingDatabase_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handler_MissingDatabase_ThrowsNotFoundException));
        var handler = new DeleteBaseDeDatosCommandHandler(db);

        var act = async () => await handler.Handle(new DeleteBaseDeDatosCommand(999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
