using System;
using AdminPro.Domain.Entities;
using AdminPro.Domain.Interfaces;
using FluentAssertions;
using Xunit;

namespace AdminPro.Application.Tests.Domain;

public class BaseDeDatosTests
{
    [Fact]
    public void SatisfiesAuditableEntityAndOwnProperties()
    {
        var db = new BaseDeDatos
        {
            Id = 1,
            ProyectoId = 1,
            Nombre = "SalesDb",
            Servidor = "SQLSRV01.corp.acme.local",
            DatabaseId = 42,
            LoginName = "app_user",
            Ambiente = "desarrollo",
            Notas = "nota",
            Activo = true,
            CreatedAt = new DateTime(2026, 1, 1),
            UpdatedAt = new DateTime(2026, 1, 2)
        };

        db.Should().BeAssignableTo<IAuditableEntity>();
        db.ProyectoId.Should().Be(1);
        db.Nombre.Should().Be("SalesDb");
        db.Servidor.Should().Be("SQLSRV01.corp.acme.local");
        db.DatabaseId.Should().Be(42);
        db.LoginName.Should().Be("app_user");
        db.Ambiente.Should().Be("desarrollo");
    }
}
