using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Ambientes.Commands.DeleteEnvironment;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AppEntity = AdminPro.Domain.Entities.Application;

namespace AdminPro.Application.Tests.Ambientes;

public class DeleteEnvironmentTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"{nameof(DeleteEnvironmentTests)}.{dbName}")
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
        var application = new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true };
        db.Applications.Add(application);
        await db.SaveChangesAsync();
        var ambiente = new Ambiente { AplicacionId = application.Id, Nombre = "UAT", Activo = true };
        db.Ambientes.Add(ambiente);
        await db.SaveChangesAsync();

        var handler = new DeleteEnvironmentCommandHandler(db);
        await handler.Handle(new DeleteEnvironmentCommand(ambiente.Id), CancellationToken.None);

        var deleted = await db.Ambientes.IgnoreQueryFilters().FirstAsync(e => e.Id == ambiente.Id);
        deleted.Activo.Should().BeFalse();
    }

    [Fact]
    public async Task Handler_MissingEnvironment_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handler_MissingEnvironment_ThrowsNotFoundException));
        var handler = new DeleteEnvironmentCommandHandler(db);

        var act = async () => await handler.Handle(new DeleteEnvironmentCommand(999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
