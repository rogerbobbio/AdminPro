using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Applications.Commands.UpdateApplication;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AppEntity = AdminPro.Domain.Entities.Application;

namespace AdminPro.Application.Tests.Applications;

public class UpdateApplicationTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"{nameof(UpdateApplicationTests)}.{dbName}")
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Validator_RejectsRenameToAnotherApplicationsNameInSameProject()
    {
        using var db = CreateInMemoryContext(nameof(Validator_RejectsRenameToAnotherApplicationsNameInSameProject));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        var crm = new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true };
        var billing = new AppEntity { ProyectoId = project.Id, Nombre = "Billing", Activo = true };
        db.Applications.AddRange(crm, billing);
        await db.SaveChangesAsync();

        var validator = new UpdateApplicationCommandValidator(db);
        var result = await validator.TestValidateAsync(
            new UpdateApplicationCommand(crm.Id, "Billing", null, null, null, null, null, null, null, null, null, 0));

        result.ShouldHaveValidationErrorFor(c => c.Nombre);
    }

    [Fact]
    public async Task Validator_AllowsRenamingToOwnCurrentName()
    {
        using var db = CreateInMemoryContext(nameof(Validator_AllowsRenamingToOwnCurrentName));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        var crm = new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true };
        db.Applications.Add(crm);
        await db.SaveChangesAsync();

        var validator = new UpdateApplicationCommandValidator(db);
        var result = await validator.TestValidateAsync(
            new UpdateApplicationCommand(crm.Id, "CRM", "Updated", null, null, null, null, null, null, null, null, 0));

        result.ShouldNotHaveValidationErrorFor(c => c.Nombre);
    }

    [Fact]
    public async Task Handler_UpdatesFields()
    {
        using var db = CreateInMemoryContext(nameof(Handler_UpdatesFields));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        var crm = new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true };
        db.Applications.Add(crm);
        await db.SaveChangesAsync();

        var handler = new UpdateApplicationCommandHandler(db);
        await handler.Handle(
            new UpdateApplicationCommand(crm.Id, "CRM Updated", "New description", null, null, null, null, null, null, null, null, 2),
            CancellationToken.None);

        var updated = await db.Applications.FindAsync(crm.Id);
        updated!.Nombre.Should().Be("CRM Updated");
        updated.Descripcion.Should().Be("New description");
        updated.Orden.Should().Be(2);
    }

    [Fact]
    public async Task Handler_MissingApplication_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handler_MissingApplication_ThrowsNotFoundException));
        var handler = new UpdateApplicationCommandHandler(db);

        var act = async () => await handler.Handle(
            new UpdateApplicationCommand(999, "CRM", null, null, null, null, null, null, null, null, null, 0),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
