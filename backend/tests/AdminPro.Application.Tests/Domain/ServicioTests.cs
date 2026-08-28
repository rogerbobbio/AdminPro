using System;
using AdminPro.Domain.Entities;
using AdminPro.Domain.Interfaces;
using FluentAssertions;
using Xunit;

namespace AdminPro.Application.Tests.Domain;

public class ServicioTests
{
    [Fact]
    public void SatisfiesAuditableEntityAndOwnProperties()
    {
        var servicio = new Servicio
        {
            Id = 1,
            ProyectoId = null,
            Nombre = "Security API UAT",
            Tipo = "Seguridad",
            Ambiente = "UAT",
            Url = "https://security-uat.example.com",
            Notas = "nota",
            EsGlobal = true,
            Activo = true,
            CreatedAt = new DateTime(2026, 1, 1),
            UpdatedAt = new DateTime(2026, 1, 2)
        };

        servicio.Should().BeAssignableTo<IAuditableEntity>();
        servicio.ProyectoId.Should().BeNull();
        servicio.Nombre.Should().Be("Security API UAT");
        servicio.EsGlobal.Should().BeTrue();
        servicio.AplicacionServicios.Should().NotBeNull().And.BeEmpty();
    }
}
