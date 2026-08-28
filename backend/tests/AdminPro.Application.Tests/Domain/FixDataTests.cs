using System;
using AdminPro.Domain.Entities;
using AdminPro.Domain.Interfaces;
using FluentAssertions;
using Xunit;

namespace AdminPro.Application.Tests.Domain;

public class FixDataTests
{
    [Fact]
    public void SatisfiesAuditableEntityAndOwnProperties()
    {
        var fixData = new FixData
        {
            Id = 1,
            AplicacionId = 1,
            Nombre = "fix-precios",
            Descripcion = "corrige precios negativos",
            Script = "UPDATE Productos SET Precio = 0 WHERE Precio < 0;",
            Orden = 0,
            Activo = true,
            CreatedAt = new DateTime(2026, 1, 1),
            UpdatedAt = new DateTime(2026, 1, 2)
        };

        fixData.Should().BeAssignableTo<IAuditableEntity>();
        fixData.AplicacionId.Should().Be(1);
        fixData.Nombre.Should().Be("fix-precios");
        fixData.Script.Should().Contain("UPDATE Productos");
    }
}
