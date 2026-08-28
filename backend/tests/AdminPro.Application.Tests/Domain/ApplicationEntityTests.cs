using System;
using AdminPro.Domain.Interfaces;
using FluentAssertions;
using Xunit;
using ApplicationEntity = AdminPro.Domain.Entities.Application;

namespace AdminPro.Application.Tests.Domain;

public class ApplicationEntityTests
{
    [Fact]
    public void SatisfiesAuditableEntityAndOwnProperties()
    {
        var app = new ApplicationEntity
        {
            Id = 1,
            ProyectoId = 1,
            Nombre = "CRM",
            Descripcion = "Customer Relationship Manager",
            TecnologiaFront = "Angular 22",
            TecnologiaBack = ".NET 10",
            RamaDesarrollo = "origin/main",
            ApplicationName = "Company.CRM",
            TieneProyectoBD = "SI",
            RutaLocal = @"C:\Dev\Acme\CRM",
            RutaGit = "https://github.com/acme/crm",
            ComoSeLevanta = "dotnet run",
            NotasCompilacion = "usar node 22",
            Orden = 0,
            Activo = true,
            CreatedAt = new DateTime(2026, 1, 1),
            UpdatedAt = new DateTime(2026, 1, 2)
        };

        app.Should().BeAssignableTo<IAuditableEntity>();
        app.ProyectoId.Should().Be(1);
        app.Nombre.Should().Be("CRM");
        app.Ambientes.Should().NotBeNull().And.BeEmpty();
        app.Reportes.Should().NotBeNull().And.BeEmpty();
        app.Notas.Should().NotBeNull().And.BeEmpty();
        app.Documentos.Should().NotBeNull().And.BeEmpty();
        app.FixDatas.Should().NotBeNull().And.BeEmpty();
        app.AplicacionServicios.Should().NotBeNull().And.BeEmpty();
    }
}
