using System;
using AdminPro.Domain.Entities;
using AdminPro.Domain.Interfaces;
using FluentAssertions;
using Xunit;

namespace AdminPro.Application.Tests.Domain;

public class NotaTests
{
    [Fact]
    public void SatisfiesAuditableEntityAndOwnProperties()
    {
        var nota = new Nota
        {
            Id = 1,
            AplicacionId = 1,
            Titulo = "nvm use 22",
            Descripcion = "Usar Node 22 para compilar",
            Orden = 0,
            Activo = true,
            CreatedAt = new DateTime(2026, 1, 1),
            UpdatedAt = new DateTime(2026, 1, 2)
        };

        nota.Should().BeAssignableTo<IAuditableEntity>();
        nota.AplicacionId.Should().Be(1);
        nota.Titulo.Should().Be("nvm use 22");
        nota.Descripcion.Should().Be("Usar Node 22 para compilar");
    }
}
