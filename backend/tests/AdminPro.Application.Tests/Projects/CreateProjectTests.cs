using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Projects.Commands.CreateProject;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdminPro.Application.Tests.Projects;

public class CreateProjectTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Validator_RejectsDuplicateName()
    {
        using var db = CreateInMemoryContext(nameof(Validator_RejectsDuplicateName));
        db.Projects.Add(new Project { Nombre = "Acme Corp", Activo = true });
        await db.SaveChangesAsync();

        var validator = new CreateProjectCommandValidator(db);
        var result = await validator.TestValidateAsync(new CreateProjectCommand("Acme Corp", null));

        result.ShouldHaveValidationErrorFor(c => c.Nombre);
    }

    [Fact]
    public async Task Validator_AcceptsUniqueName()
    {
        using var db = CreateInMemoryContext(nameof(Validator_AcceptsUniqueName));

        var validator = new CreateProjectCommandValidator(db);
        var result = await validator.TestValidateAsync(new CreateProjectCommand("Globex Corp", "Nuevo cliente"));

        result.ShouldNotHaveValidationErrorFor(c => c.Nombre);
    }

    [Fact]
    public async Task Handler_CreatesProjectAndReturnsId()
    {
        using var db = CreateInMemoryContext(nameof(Handler_CreatesProjectAndReturnsId));
        var handler = new CreateProjectCommandHandler(db);

        var id = await handler.Handle(new CreateProjectCommand("Globex Corp", "Nuevo cliente"), CancellationToken.None);

        var project = await db.Projects.FindAsync(id);
        project.Should().NotBeNull();
        project!.Nombre.Should().Be("Globex Corp");
        project.Descripcion.Should().Be("Nuevo cliente");
        project.Activo.Should().BeTrue();
    }
}
