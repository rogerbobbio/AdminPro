using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Application.Reportes.Commands.CreateReporte;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AppEntity = AdminPro.Domain.Entities.Application;

namespace AdminPro.Application.Tests.Reportes;

public class CreateReporteTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"{nameof(CreateReporteTests)}.{dbName}")
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Validator_RejectsDuplicateReportCodeWithinSameApplication()
    {
        using var db = CreateInMemoryContext(nameof(Validator_RejectsDuplicateReportCodeWithinSameApplication));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();
        var application = new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true };
        db.Applications.Add(application);
        await db.SaveChangesAsync();
        db.Reportes.Add(new Reporte { AplicacionId = application.Id, ReportCode = "VFL", ReportName = "Volumen de Carga", Activo = true });
        await db.SaveChangesAsync();

        var validator = new CreateReporteCommandValidator(db);
        var result = await validator.TestValidateAsync(
            new CreateReporteCommand(application.Id, "VFL", "Volumen de Carga v2", null, null, null, null, null, null));

        result.ShouldHaveValidationErrorFor(c => c.ReportCode);
    }

    [Fact]
    public async Task Handler_CreatesReporteScopedToApplication()
    {
        using var db = CreateInMemoryContext(nameof(Handler_CreatesReporteScopedToApplication));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();
        var application = new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true };
        db.Applications.Add(application);
        await db.SaveChangesAsync();

        var handler = new CreateReporteCommandHandler(db);
        var id = await handler.Handle(
            new CreateReporteCommand(application.Id, "AUT", "Autorizaciones", null, null, null, null, null, null),
            CancellationToken.None);

        var created = await db.Reportes.FindAsync(id);
        created!.ReportCode.Should().Be("AUT");
        created.AplicacionId.Should().Be(application.Id);
    }

    [Fact]
    public async Task Handler_MissingApplication_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handler_MissingApplication_ThrowsNotFoundException));
        var handler = new CreateReporteCommandHandler(db);

        var act = async () => await handler.Handle(
            new CreateReporteCommand(999, "AUT", "Autorizaciones", null, null, null, null, null, null),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
