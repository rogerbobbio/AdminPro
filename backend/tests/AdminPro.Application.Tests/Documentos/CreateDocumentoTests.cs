using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Application.Documentos.Commands.CreateDocumento;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AppEntity = AdminPro.Domain.Entities.Application;

namespace AdminPro.Application.Tests.Documentos;

public class CreateDocumentoTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"{nameof(CreateDocumentoTests)}.{dbName}")
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public void Validator_RejectsInvalidUrl()
    {
        var validator = new CreateDocumentoCommandValidator();
        var result = validator.TestValidate(new CreateDocumentoCommand(1, "Manual", "not-a-url", "manual", null, 0));

        result.ShouldHaveValidationErrorFor(c => c.UrlOneDrive);
    }

    [Fact]
    public void Validator_AcceptsValidUrl()
    {
        var validator = new CreateDocumentoCommandValidator();
        var result = validator.TestValidate(new CreateDocumentoCommand(1, "Manual", "https://onedrive.example.com/manual", "manual", null, 0));

        result.ShouldNotHaveValidationErrorFor(c => c.UrlOneDrive);
    }

    [Fact]
    public async Task Handler_CreatesDocumentoScopedToApplication()
    {
        using var db = CreateInMemoryContext(nameof(Handler_CreatesDocumentoScopedToApplication));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();
        var application = new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true };
        db.Applications.Add(application);
        await db.SaveChangesAsync();

        var handler = new CreateDocumentoCommandHandler(db);
        var id = await handler.Handle(
            new CreateDocumentoCommand(application.Id, "Manual de Usuario", "https://onedrive.example.com/manual", "manual", null, 0),
            CancellationToken.None);

        var created = await db.Documentos.FindAsync(id);
        created!.NombreArchivo.Should().Be("Manual de Usuario");
        created.AplicacionId.Should().Be(application.Id);
    }

    [Fact]
    public async Task Handler_MissingApplication_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handler_MissingApplication_ThrowsNotFoundException));
        var handler = new CreateDocumentoCommandHandler(db);

        var act = async () => await handler.Handle(
            new CreateDocumentoCommand(999, "Manual", "https://onedrive.example.com/manual", "manual", null, 0),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
