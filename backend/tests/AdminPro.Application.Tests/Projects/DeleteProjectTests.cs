using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Application.Projects.Commands.DeleteProject;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AppEntity = AdminPro.Domain.Entities.Application;

namespace AdminPro.Application.Tests.Projects;

public class DeleteProjectTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handle_DeactivatesProjectAndCascadesToChildren()
    {
        using var db = CreateInMemoryContext(nameof(Handle_DeactivatesProjectAndCascadesToChildren));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        db.BasesDeDatos.Add(new BaseDeDatos { ProyectoId = project.Id, Nombre = "SalesDb", Activo = true });
        db.Applications.Add(new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true });
        await db.SaveChangesAsync();

        var handler = new DeleteProjectCommandHandler(db);
        await handler.Handle(new DeleteProjectCommand(project.Id), CancellationToken.None);

        (await db.Projects.IgnoreQueryFilters().FirstAsync(p => p.Id == project.Id)).Activo.Should().BeFalse();
        (await db.BasesDeDatos.IgnoreQueryFilters().Where(d => d.ProyectoId == project.Id).ToListAsync())
            .Should().OnlyContain(d => !d.Activo);
        (await db.Applications.IgnoreQueryFilters().Where(a => a.ProyectoId == project.Id).ToListAsync())
            .Should().OnlyContain(a => !a.Activo);
    }

    [Fact]
    public async Task Handle_MissingProject_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handle_MissingProject_ThrowsNotFoundException));
        var handler = new DeleteProjectCommandHandler(db);

        var act = async () => await handler.Handle(new DeleteProjectCommand(999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
