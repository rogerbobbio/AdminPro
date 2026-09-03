using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Application.Notas.Commands.CreateNota;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AppEntity = AdminPro.Domain.Entities.Application;

namespace AdminPro.Application.Tests.Notas;

public class CreateNotaTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"{nameof(CreateNotaTests)}.{dbName}")
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public void Validator_RejectsEmptyTitulo()
    {
        var validator = new CreateNotaCommandValidator();
        var result = validator.TestValidate(new CreateNotaCommand(1, "", "Some memo", 0));

        result.ShouldHaveValidationErrorFor(c => c.Titulo);
    }

    [Fact]
    public void Validator_RejectsEmptyDescripcion()
    {
        var validator = new CreateNotaCommandValidator();
        var result = validator.TestValidate(new CreateNotaCommand(1, "nvm use 14.16.0", "", 0));

        result.ShouldHaveValidationErrorFor(c => c.Descripcion);
    }

    [Fact]
    public async Task Handler_CreatesNotaScopedToApplication()
    {
        using var db = CreateInMemoryContext(nameof(Handler_CreatesNotaScopedToApplication));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();
        var application = new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true };
        db.Applications.Add(application);
        await db.SaveChangesAsync();

        var handler = new CreateNotaCommandHandler(db);
        var id = await handler.Handle(
            new CreateNotaCommand(application.Id, "nvm use 14.16.0", "Usar Node 14.16.0 para compilar el front.", 0),
            CancellationToken.None);

        var created = await db.Notas.FindAsync(id);
        created!.Titulo.Should().Be("nvm use 14.16.0");
        created.AplicacionId.Should().Be(application.Id);
    }

    [Fact]
    public async Task Handler_MissingApplication_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handler_MissingApplication_ThrowsNotFoundException));
        var handler = new CreateNotaCommandHandler(db);

        var act = async () => await handler.Handle(
            new CreateNotaCommand(999, "nvm use 14.16.0", "Usar Node 14.16.0.", 0),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
