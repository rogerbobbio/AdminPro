using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Application.Notas.Commands.UpdateNota;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AppEntity = AdminPro.Domain.Entities.Application;

namespace AdminPro.Application.Tests.Notas;

public class UpdateNotaTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"{nameof(UpdateNotaTests)}.{dbName}")
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
        var application = new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true };
        db.Applications.Add(application);
        await db.SaveChangesAsync();
        var nota = new Nota { AplicacionId = application.Id, Titulo = "Borrar bin/obj", Descripcion = "Antes de compilar.", Activo = true };
        db.Notas.Add(nota);
        await db.SaveChangesAsync();

        var handler = new UpdateNotaCommandHandler(db);
        await handler.Handle(
            new UpdateNotaCommand(nota.Id, "Borrar bin/obj", "Antes de compilar, borrar las carpetas bin y obj.", 1),
            CancellationToken.None);

        var updated = await db.Notas.FindAsync(nota.Id);
        updated!.Descripcion.Should().Be("Antes de compilar, borrar las carpetas bin y obj.");
        updated.Orden.Should().Be(1);
    }

    [Fact]
    public async Task Handler_MissingNota_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handler_MissingNota_ThrowsNotFoundException));
        var handler = new UpdateNotaCommandHandler(db);

        var act = async () => await handler.Handle(
            new UpdateNotaCommand(999, "Titulo", "Descripcion", 0),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
