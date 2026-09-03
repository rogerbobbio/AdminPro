using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Application.Documentos.Commands.DeleteDocumento;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AppEntity = AdminPro.Domain.Entities.Application;

namespace AdminPro.Application.Tests.Documentos;

public class DeleteDocumentoTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"{nameof(DeleteDocumentoTests)}.{dbName}")
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
        var documento = new Documento { AplicacionId = application.Id, NombreArchivo = "Manual", UrlOneDrive = "https://onedrive.example.com/manual", Tipo = "manual", Activo = true };
        db.Documentos.Add(documento);
        await db.SaveChangesAsync();

        var handler = new DeleteDocumentoCommandHandler(db);
        await handler.Handle(new DeleteDocumentoCommand(documento.Id), CancellationToken.None);

        var deleted = await db.Documentos.IgnoreQueryFilters().FirstAsync(d => d.Id == documento.Id);
        deleted.Activo.Should().BeFalse();
    }

    [Fact]
    public async Task Handler_MissingDocumento_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handler_MissingDocumento_ThrowsNotFoundException));
        var handler = new DeleteDocumentoCommandHandler(db);

        var act = async () => await handler.Handle(new DeleteDocumentoCommand(999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
