using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Applications.Commands.CreateApplication;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AppEntity = AdminPro.Domain.Entities.Application;

namespace AdminPro.Application.Tests.Applications;

public class CreateApplicationTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"{nameof(CreateApplicationTests)}.{dbName}")
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Validator_RejectsDuplicateNameWithinSameProject()
    {
        using var db = CreateInMemoryContext(nameof(Validator_RejectsDuplicateNameWithinSameProject));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();
        db.Applications.Add(new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true });
        await db.SaveChangesAsync();

        var validator = new CreateApplicationCommandValidator(db);
        var result = await validator.TestValidateAsync(
            new CreateApplicationCommand(project.Id, "CRM", null, null, null, null, null, null, null, null, null, null, 0));

        result.ShouldHaveValidationErrorFor(c => c.Nombre);
    }

    [Fact]
    public async Task Validator_AllowsSameNameInDifferentProject()
    {
        using var db = CreateInMemoryContext(nameof(Validator_AllowsSameNameInDifferentProject));
        var acme = new Project { Nombre = "Acme Corp", Activo = true };
        var globex = new Project { Nombre = "Globex Corp", Activo = true };
        db.Projects.AddRange(acme, globex);
        await db.SaveChangesAsync();
        db.Applications.Add(new AppEntity { ProyectoId = acme.Id, Nombre = "CRM", Activo = true });
        await db.SaveChangesAsync();

        var validator = new CreateApplicationCommandValidator(db);
        var result = await validator.TestValidateAsync(
            new CreateApplicationCommand(globex.Id, "CRM", null, null, null, null, null, null, null, null, null, null, 0));

        result.ShouldNotHaveValidationErrorFor(c => c.Nombre);
    }

    [Fact]
    public async Task Handler_CreatesApplicationScopedToProject()
    {
        using var db = CreateInMemoryContext(nameof(Handler_CreatesApplicationScopedToProject));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        var handler = new CreateApplicationCommandHandler(db);
        var id = await handler.Handle(
            new CreateApplicationCommand(project.Id, "CRM", "Customer Relationship Manager", null, "Angular", ".NET", null, null, null, null, null, null, 1),
            CancellationToken.None);

        var created = await db.Applications.FindAsync(id);
        created!.Nombre.Should().Be("CRM");
        created.ProyectoId.Should().Be(project.Id);
        created.TecnologiaFront.Should().Be("Angular");
        created.Orden.Should().Be(1);
    }

    [Fact]
    public async Task Handler_MissingProject_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handler_MissingProject_ThrowsNotFoundException));
        var handler = new CreateApplicationCommandHandler(db);

        var act = async () => await handler.Handle(
            new CreateApplicationCommand(999, "CRM", null, null, null, null, null, null, null, null, null, null, 0),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
