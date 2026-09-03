using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Application.Reportes.Commands.DeleteReporte;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AppEntity = AdminPro.Domain.Entities.Application;

namespace AdminPro.Application.Tests.Reportes;

public class DeleteReporteTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"{nameof(DeleteReporteTests)}.{dbName}")
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handler_SetsActivoFalse()
    {
        using var db = CreateInMemoryContext(nameof(Handler_SetsActivoFalse));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();
        var application = new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true };
        db.Applications.Add(application);
        await db.SaveChangesAsync();
        var reporte = new Reporte { AplicacionId = application.Id, ReportCode = "VFL", ReportName = "Volumen de Carga", Activo = true };
        db.Reportes.Add(reporte);
        await db.SaveChangesAsync();

        var handler = new DeleteReporteCommandHandler(db);
        await handler.Handle(new DeleteReporteCommand(reporte.Id), CancellationToken.None);

        var deleted = await db.Reportes.IgnoreQueryFilters().FirstAsync(r => r.Id == reporte.Id);
        deleted.Activo.Should().BeFalse();
    }

    [Fact]
    public async Task Handler_MissingReporte_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handler_MissingReporte_ThrowsNotFoundException));
        var handler = new DeleteReporteCommandHandler(db);

        var act = async () => await handler.Handle(new DeleteReporteCommand(999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
