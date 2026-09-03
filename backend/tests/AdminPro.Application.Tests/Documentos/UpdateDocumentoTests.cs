using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Application.Documentos.Commands.UpdateDocumento;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AppEntity = AdminPro.Domain.Entities.Application;

namespace AdminPro.Application.Tests.Documentos;

public class UpdateDocumentoTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"{nameof(UpdateDocumentoTests)}.{dbName}")
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
        var documento = new Documento { AplicacionId = application.Id, NombreArchivo = "Manual", UrlOneDrive = "https://onedrive.example.com/manual", Tipo = "manual", Activo = true };
        db.Documentos.Add(documento);
        await db.SaveChangesAsync();

        var handler = new UpdateDocumentoCommandHandler(db);
        await handler.Handle(
            new UpdateDocumentoCommand(documento.Id, "Manual v2", "https://onedrive.example.com/manual-v2", "manual", "Version actualizada", 1),
            CancellationToken.None);

        var updated = await db.Documentos.FindAsync(documento.Id);
        updated!.NombreArchivo.Should().Be("Manual v2");
        updated.UrlOneDrive.Should().Be("https://onedrive.example.com/manual-v2");
    }

    [Fact]
    public async Task Handler_MissingDocumento_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handler_MissingDocumento_ThrowsNotFoundException));
        var handler = new UpdateDocumentoCommandHandler(db);

        var act = async () => await handler.Handle(
            new UpdateDocumentoCommand(999, "Manual", "https://onedrive.example.com/manual", "manual", null, 0),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
