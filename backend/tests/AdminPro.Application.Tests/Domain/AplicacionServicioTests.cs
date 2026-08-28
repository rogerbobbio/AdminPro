using System;
using AdminPro.Domain.Entities;
using AdminPro.Domain.Interfaces;
using FluentAssertions;
using Xunit;

namespace AdminPro.Application.Tests.Domain;

public class AplicacionServicioTests
{
    [Fact]
    public void HasCompositeKeyShapeAndIsNotAuditable()
    {
        var link = new AplicacionServicio
        {
            AplicacionId = 1,
            ServicioId = 2,
            NotasEspecificas = "uso en checkout",
            CreatedAt = new DateTime(2026, 1, 1)
        };

        link.AplicacionId.Should().Be(1);
        link.ServicioId.Should().Be(2);
        link.NotasEspecificas.Should().Be("uso en checkout");
        link.Should().NotBeAssignableTo<IAuditableEntity>();
    }
}
