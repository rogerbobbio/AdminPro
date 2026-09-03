using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Applications.Queries.GetApplicationById;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AppEntity = AdminPro.Domain.Entities.Application;

namespace AdminPro.Application.Tests.Applications;

public class GetApplicationByIdQueryHandlerTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"{nameof(GetApplicationByIdQueryHandlerTests)}.{dbName}")
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handle_ReturnsDetailWithAmbientesAndEmptyChildCollections()
    {
        using var db = CreateInMemoryContext(nameof(Handle_ReturnsDetailWithAmbientesAndEmptyChildCollections));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        var application = new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true };
        db.Applications.Add(application);
        await db.SaveChangesAsync();

        db.Ambientes.Add(new Ambiente { AplicacionId = application.Id, Nombre = "UAT", Activo = true });
        await db.SaveChangesAsync();

        var handler = new GetApplicationByIdQueryHandler(db);
        var result = await handler.Handle(new GetApplicationByIdQuery(application.Id), CancellationToken.None);

        result.Nombre.Should().Be("CRM");
        result.Ambientes.Should().HaveCount(1);
        result.Reportes.Should().BeEmpty();
        result.Notas.Should().BeEmpty();
        result.Documentos.Should().BeEmpty();
        result.FixDatas.Should().BeEmpty();
        result.Servicios.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ReturnsRealReportesNotasDocumentosFixDatas()
    {
        using var db = CreateInMemoryContext(nameof(Handle_ReturnsRealReportesNotasDocumentosFixDatas));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        var application = new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true };
        db.Applications.Add(application);
        await db.SaveChangesAsync();

        db.Reportes.Add(new Reporte { AplicacionId = application.Id, ReportCode = "VFL", ReportName = "Volumen de Carga", Activo = true });
        db.Notas.Add(new Nota { AplicacionId = application.Id, Titulo = "nvm use 14.16.0", Descripcion = "Usar Node 14.16.0.", Activo = true });
        db.Documentos.Add(new Documento { AplicacionId = application.Id, NombreArchivo = "Manual", UrlOneDrive = "https://onedrive.example.com/manual", Tipo = "manual", Activo = true });
        db.FixDatas.Add(new FixData { AplicacionId = application.Id, Nombre = "Fix duplicate customers", Activo = true });
        await db.SaveChangesAsync();

        var handler = new GetApplicationByIdQueryHandler(db);
        var result = await handler.Handle(new GetApplicationByIdQuery(application.Id), CancellationToken.None);

        result.Reportes.Should().ContainSingle(r => r.ReportCode == "VFL");
        result.Notas.Should().ContainSingle(n => n.Titulo == "nvm use 14.16.0");
        result.Documentos.Should().ContainSingle(d => d.NombreArchivo == "Manual");
        result.FixDatas.Should().ContainSingle(f => f.Nombre == "Fix duplicate customers");
    }

    [Fact]
    public async Task Handle_MissingApplication_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handle_MissingApplication_ThrowsNotFoundException));
        var handler = new GetApplicationByIdQueryHandler(db);

        var act = async () => await handler.Handle(new GetApplicationByIdQuery(999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
