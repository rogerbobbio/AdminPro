using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Application.FixDatas.Commands.CreateFixData;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AppEntity = AdminPro.Domain.Entities.Application;
using ProjectEntity = AdminPro.Domain.Entities.Project;

namespace AdminPro.Application.Tests.FixDatas;

public class CreateFixDataTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"{nameof(CreateFixDataTests)}.{dbName}")
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public void Validator_RejectsEmptyNombre()
    {
        var validator = new CreateFixDataCommandValidator();
        var result = validator.TestValidate(new CreateFixDataCommand(1, "", null, null, 0));

        result.ShouldHaveValidationErrorFor(c => c.Nombre);
    }

    [Fact]
    public async Task Handler_CreatesFixDataScopedToApplication()
    {
        using var db = CreateInMemoryContext(nameof(Handler_CreatesFixDataScopedToApplication));
        var project = new ProjectEntity { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();
        var application = new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true };
        db.Applications.Add(application);
        await db.SaveChangesAsync();

        var handler = new CreateFixDataCommandHandler(db);
        var id = await handler.Handle(
            new CreateFixDataCommand(application.Id, "Fix duplicate customers", null, "DELETE FROM Customers WHERE Id > 100;", 0),
            CancellationToken.None);

        var created = await db.FixDatas.FindAsync(id);
        created!.Nombre.Should().Be("Fix duplicate customers");
        created.AplicacionId.Should().Be(application.Id);
    }

    [Fact]
    public async Task Handler_MissingApplication_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handler_MissingApplication_ThrowsNotFoundException));
        var handler = new CreateFixDataCommandHandler(db);

        var act = async () => await handler.Handle(
            new CreateFixDataCommand(999, "Fix duplicate customers", null, null, 0),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
