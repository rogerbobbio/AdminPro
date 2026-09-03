using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Application.FixDatas.Commands.UpdateFixData;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AppEntity = AdminPro.Domain.Entities.Application;
using FixDataEntity = AdminPro.Domain.Entities.FixData;
using ProjectEntity = AdminPro.Domain.Entities.Project;

namespace AdminPro.Application.Tests.FixDatas;

public class UpdateFixDataTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"{nameof(UpdateFixDataTests)}.{dbName}")
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handler_UpdatesFields()
    {
        using var db = CreateInMemoryContext(nameof(Handler_UpdatesFields));
        var project = new ProjectEntity { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();
        var application = new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true };
        db.Applications.Add(application);
        await db.SaveChangesAsync();
        var fixData = new FixDataEntity { AplicacionId = application.Id, Nombre = "Fix duplicate customers", Activo = true };
        db.FixDatas.Add(fixData);
        await db.SaveChangesAsync();

        var handler = new UpdateFixDataCommandHandler(db);
        await handler.Handle(
            new UpdateFixDataCommand(fixData.Id, "Fix duplicate customers v2", "Updated", "DELETE FROM ...", 1),
            CancellationToken.None);

        var updated = await db.FixDatas.FindAsync(fixData.Id);
        updated!.Nombre.Should().Be("Fix duplicate customers v2");
        updated.Script.Should().Be("DELETE FROM ...");
    }

    [Fact]
    public async Task Handler_MissingFixData_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handler_MissingFixData_ThrowsNotFoundException));
        var handler = new UpdateFixDataCommandHandler(db);

        var act = async () => await handler.Handle(
            new UpdateFixDataCommand(999, "Fix", null, null, 0),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
