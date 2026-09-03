using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Application.FixDatas.Commands.DeleteFixData;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AppEntity = AdminPro.Domain.Entities.Application;
using FixDataEntity = AdminPro.Domain.Entities.FixData;
using ProjectEntity = AdminPro.Domain.Entities.Project;

namespace AdminPro.Application.Tests.FixDatas;

public class DeleteFixDataTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"{nameof(DeleteFixDataTests)}.{dbName}")
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handler_SetsActivoFalse()
    {
        using var db = CreateInMemoryContext(nameof(Handler_SetsActivoFalse));
        var project = new ProjectEntity { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();
        var application = new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true };
        db.Applications.Add(application);
        await db.SaveChangesAsync();
        var fixData = new FixDataEntity { AplicacionId = application.Id, Nombre = "Fix duplicate customers", Activo = true };
        db.FixDatas.Add(fixData);
        await db.SaveChangesAsync();

        var handler = new DeleteFixDataCommandHandler(db);
        await handler.Handle(new DeleteFixDataCommand(fixData.Id), CancellationToken.None);

        var deleted = await db.FixDatas.IgnoreQueryFilters().FirstAsync(f => f.Id == fixData.Id);
        deleted.Activo.Should().BeFalse();
    }

    [Fact]
    public async Task Handler_MissingFixData_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handler_MissingFixData_ThrowsNotFoundException));
        var handler = new DeleteFixDataCommandHandler(db);

        var act = async () => await handler.Handle(new DeleteFixDataCommand(999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
