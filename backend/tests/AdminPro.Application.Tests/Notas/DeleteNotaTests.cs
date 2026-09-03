using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Application.Notas.Commands.DeleteNota;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AppEntity = AdminPro.Domain.Entities.Application;

namespace AdminPro.Application.Tests.Notas;

public class DeleteNotaTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"{nameof(DeleteNotaTests)}.{dbName}")
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
        var nota = new Nota { AplicacionId = application.Id, Titulo = "Titulo", Descripcion = "Descripcion", Activo = true };
        db.Notas.Add(nota);
        await db.SaveChangesAsync();

        var handler = new DeleteNotaCommandHandler(db);
        await handler.Handle(new DeleteNotaCommand(nota.Id), CancellationToken.None);

        var deleted = await db.Notas.IgnoreQueryFilters().FirstAsync(n => n.Id == nota.Id);
        deleted.Activo.Should().BeFalse();
    }

    [Fact]
    public async Task Handler_MissingNota_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handler_MissingNota_ThrowsNotFoundException));
        var handler = new DeleteNotaCommandHandler(db);

        var act = async () => await handler.Handle(new DeleteNotaCommand(999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
