using System;
using AdminPro.Domain.Entities;
using AdminPro.Domain.Interfaces;
using FluentAssertions;
using Xunit;

namespace AdminPro.Application.Tests.Domain;

public class AmbienteTests
{
    [Fact]
    public void SatisfiesAuditableEntityAndOwnProperties()
    {
        var ambiente = new Ambiente
        {
            Id = 1,
            AplicacionId = 1,
            Nombre = "UAT",
            Url = "https://uat.example.com",
            EsWebApi = false,
            Notas = "requiere VPN",
            Orden = 0,
            Activo = true,
            CreatedAt = new DateTime(2026, 1, 1),
            UpdatedAt = new DateTime(2026, 1, 2)
        };

        ambiente.Should().BeAssignableTo<IAuditableEntity>();
        ambiente.AplicacionId.Should().Be(1);
        ambiente.Nombre.Should().Be("UAT");
        ambiente.Url.Should().Be("https://uat.example.com");
        ambiente.EsWebApi.Should().BeFalse();
    }
}
