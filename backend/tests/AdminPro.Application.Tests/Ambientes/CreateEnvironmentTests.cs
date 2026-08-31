using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Ambientes.Commands.CreateEnvironment;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AppEntity = AdminPro.Domain.Entities.Application;

namespace AdminPro.Application.Tests.Ambientes;

public class CreateEnvironmentTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"{nameof(CreateEnvironmentTests)}.{dbName}")
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public void Validator_RejectsInvalidUrl()
    {
        var validator = new CreateEnvironmentCommandValidator();
        var result = validator.TestValidate(new CreateEnvironmentCommand(1, "UAT", "not-a-url", false, null, 0));

        result.ShouldHaveValidationErrorFor(c => c.Url);
    }

    [Fact]
    public void Validator_AcceptsValidUrl()
    {
        var validator = new CreateEnvironmentCommandValidator();
        var result = validator.TestValidate(new CreateEnvironmentCommand(1, "UAT", "https://uat.example.com", false, null, 0));

        result.ShouldNotHaveValidationErrorFor(c => c.Url);
    }

    [Fact]
    public async Task Handler_CreatesEnvironmentScopedToApplication()
    {
        using var db = CreateInMemoryContext(nameof(Handler_CreatesEnvironmentScopedToApplication));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();
        var application = new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true };
        db.Applications.Add(application);
        await db.SaveChangesAsync();

        var handler = new CreateEnvironmentCommandHandler(db);
        var id = await handler.Handle(
            new CreateEnvironmentCommand(application.Id, "UAT", "https://uat.example.com", false, null, 0),
            CancellationToken.None);

        var created = await db.Ambientes.FindAsync(id);
        created!.Nombre.Should().Be("UAT");
        created.AplicacionId.Should().Be(application.Id);
    }

    [Fact]
    public async Task Handler_MissingApplication_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handler_MissingApplication_ThrowsNotFoundException));
        var handler = new CreateEnvironmentCommandHandler(db);

        var act = async () => await handler.Handle(
            new CreateEnvironmentCommand(999, "UAT", null, false, null, 0),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
