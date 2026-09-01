using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Applications.Commands.DeleteApplication;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AppEntity = AdminPro.Domain.Entities.Application;

namespace AdminPro.Application.Tests.Applications;

public class DeleteApplicationTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"{nameof(DeleteApplicationTests)}.{dbName}")
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handle_DeactivatesApplicationAndCascadesToEnvironments()
    {
        using var db = CreateInMemoryContext(nameof(Handle_DeactivatesApplicationAndCascadesToEnvironments));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        var application = new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true };
        db.Applications.Add(application);
        await db.SaveChangesAsync();

        db.Ambientes.AddRange(
            new Ambiente { AplicacionId = application.Id, Nombre = "UAT", Activo = true },
            new Ambiente { AplicacionId = application.Id, Nombre = "PROD", Activo = true });
        await db.SaveChangesAsync();

        var handler = new DeleteApplicationCommandHandler(db);
        await handler.Handle(new DeleteApplicationCommand(application.Id), CancellationToken.None);

        (await db.Applications.IgnoreQueryFilters().FirstAsync(a => a.Id == application.Id)).Activo.Should().BeFalse();
        (await db.Ambientes.IgnoreQueryFilters().Where(e => e.AplicacionId == application.Id).ToListAsync())
            .Should().OnlyContain(e => !e.Activo);
    }

    [Fact]
    public async Task Handle_MissingApplication_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handle_MissingApplication_ThrowsNotFoundException));
        var handler = new DeleteApplicationCommandHandler(db);

        var act = async () => await handler.Handle(new DeleteApplicationCommand(999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
