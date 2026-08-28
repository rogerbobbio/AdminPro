using System;
using AdminPro.Domain.Entities;
using AdminPro.Domain.Interfaces;
using FluentAssertions;
using Xunit;

namespace AdminPro.Application.Tests.Domain;

public class ProjectTests
{
    [Fact]
    public void SatisfiesAuditableEntityAndOwnProperties()
    {
        var project = new Project
        {
            Id = 1,
            Nombre = "Acme Corp",
            Descripcion = "Cliente principal",
            Activo = true,
            CreatedAt = new DateTime(2026, 1, 1),
            UpdatedAt = new DateTime(2026, 1, 2)
        };

        project.Should().BeAssignableTo<IAuditableEntity>();
        project.Nombre.Should().Be("Acme Corp");
        project.Descripcion.Should().Be("Cliente principal");
        project.BasesDeDatos.Should().NotBeNull().And.BeEmpty();
        project.Applications.Should().NotBeNull().And.BeEmpty();
    }
}
