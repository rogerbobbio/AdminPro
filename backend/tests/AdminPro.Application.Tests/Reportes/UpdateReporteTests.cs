using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Application.Reportes.Commands.UpdateReporte;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AppEntity = AdminPro.Domain.Entities.Application;

namespace AdminPro.Application.Tests.Reportes;

public class UpdateReporteTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"{nameof(UpdateReporteTests)}.{dbName}")
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Validator_RejectsRenameToAnotherReportsCodeInSameApplication()
    {
        using var db = CreateInMemoryContext(nameof(Validator_RejectsRenameToAnotherReportsCodeInSameApplication));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();
        var application = new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true };
        db.Applications.Add(application);
        await db.SaveChangesAsync();
        var vfl = new Reporte { AplicacionId = application.Id, ReportCode = "VFL", ReportName = "Volumen de Carga", Activo = true };
        var aut = new Reporte { AplicacionId = application.Id, ReportCode = "AUT", ReportName = "Autorizaciones", Activo = true };
        db.Reportes.AddRange(vfl, aut);
        await db.SaveChangesAsync();

        var validator = new UpdateReporteCommandValidator(db);
        var result = await validator.TestValidateAsync(
            new UpdateReporteCommand(vfl.Id, "AUT", "Volumen de Carga", null, null, null, null, null, null));

        result.ShouldHaveValidationErrorFor(c => c.ReportCode);
    }

    [Fact]
    public async Task Handler_UpdatesFields()
    {
        using var db = CreateInMemoryContext(nameof(Handler_UpdatesFields));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();
        var application = new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true };
        db.Applications.Add(application);
        await db.SaveChangesAsync();
        var reporte = new Reporte { AplicacionId = application.Id, ReportCode = "VFL", ReportName = "Volumen de Carga", Activo = true };
        db.Reportes.Add(reporte);
        await db.SaveChangesAsync();

        var handler = new UpdateReporteCommandHandler(db);
        await handler.Handle(
            new UpdateReporteCommand(reporte.Id, "VFL", "Volumen de Carga Actualizado", "DLA", null, null, null, null, null),
            CancellationToken.None);

        var updated = await db.Reportes.FindAsync(reporte.Id);
        updated!.ReportName.Should().Be("Volumen de Carga Actualizado");
        updated.RegionId.Should().Be("DLA");
    }

    [Fact]
    public async Task Handler_MissingReporte_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handler_MissingReporte_ThrowsNotFoundException));
        var handler = new UpdateReporteCommandHandler(db);

        var act = async () => await handler.Handle(
            new UpdateReporteCommand(999, "VFL", "Volumen de Carga", null, null, null, null, null, null),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
