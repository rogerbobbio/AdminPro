using System;
using AdminPro.Domain.Entities;
using AdminPro.Domain.Interfaces;
using FluentAssertions;
using Xunit;

namespace AdminPro.Application.Tests.Domain;

public class ModuloTests
{
    [Fact]
    public void SatisfiesAuditableEntityAndOwnProperties()
    {
        var createdAt = new DateTime(2026, 1, 1);
        var updatedAt = new DateTime(2026, 1, 2);

        var modulo = new Modulo
        {
            Id = 1,
            Nombre = "Gestión de Proyectos",
            Icono = "bi-kanban",
            RutaBase = "proyectos",
            Color = "primary",
            Orden = 0,
            Activo = true,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        modulo.Should().BeAssignableTo<IAuditableEntity>();
        modulo.Nombre.Should().Be("Gestión de Proyectos");
        modulo.Icono.Should().Be("bi-kanban");
        modulo.RutaBase.Should().Be("proyectos");
        modulo.Color.Should().Be("primary");
        modulo.Orden.Should().Be(0);
        modulo.Activo.Should().BeTrue();
        modulo.CreatedAt.Should().Be(createdAt);
        modulo.UpdatedAt.Should().Be(updatedAt);
    }
}
