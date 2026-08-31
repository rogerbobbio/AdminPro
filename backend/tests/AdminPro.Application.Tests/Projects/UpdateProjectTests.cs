using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Application.Projects.Commands.UpdateProject;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdminPro.Application.Tests.Projects;

public class UpdateProjectTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Validator_RejectsRenameToAnotherProjectsName()
    {
        using var db = CreateInMemoryContext(nameof(Validator_RejectsRenameToAnotherProjectsName));
        var acme = new Project { Nombre = "Acme Corp", Activo = true };
        var globex = new Project { Nombre = "Globex Corp", Activo = true };
        db.Projects.AddRange(acme, globex);
        await db.SaveChangesAsync();

        var validator = new UpdateProjectCommandValidator(db);
        var result = await validator.TestValidateAsync(new UpdateProjectCommand(acme.Id, "Globex Corp", null));

        result.ShouldHaveValidationErrorFor(c => c.Nombre);
    }

    [Fact]
    public async Task Validator_AllowsRenamingToOwnCurrentName()
    {
        using var db = CreateInMemoryContext(nameof(Validator_AllowsRenamingToOwnCurrentName));
        var acme = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(acme);
        await db.SaveChangesAsync();

        var validator = new UpdateProjectCommandValidator(db);
        var result = await validator.TestValidateAsync(new UpdateProjectCommand(acme.Id, "Acme Corp", "Updated"));

        result.ShouldNotHaveValidationErrorFor(c => c.Nombre);
    }

    [Fact]
    public async Task Handler_UpdatesFields()
    {
        using var db = CreateInMemoryContext(nameof(Handler_UpdatesFields));
        var acme = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(acme);
        await db.SaveChangesAsync();

        var handler = new UpdateProjectCommandHandler(db);
        await handler.Handle(new UpdateProjectCommand(acme.Id, "Acme Corp Updated", "New description"), CancellationToken.None);

        var updated = await db.Projects.FindAsync(acme.Id);
        updated!.Nombre.Should().Be("Acme Corp Updated");
        updated.Descripcion.Should().Be("New description");
    }

    [Fact]
    public async Task Handler_MissingProject_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handler_MissingProject_ThrowsNotFoundException));
        var handler = new UpdateProjectCommandHandler(db);

        var act = async () => await handler.Handle(new UpdateProjectCommand(999, "Acme Corp", null), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
