using System;
using AdminPro.Domain.Entities;
using AdminPro.Domain.Interfaces;
using FluentAssertions;
using Xunit;

namespace AdminPro.Application.Tests.Domain;

public class DocumentoTests
{
    [Fact]
    public void SatisfiesAuditableEntityAndOwnProperties()
    {
        var documento = new Documento
        {
            Id = 1,
            AplicacionId = 1,
            NombreArchivo = "Manual de Usuario",
            UrlOneDrive = "https://onedrive.example.com/manual",
            Tipo = "manual",
            Descripcion = "Manual de usuario v1",
            Orden = 0,
            Activo = true,
            CreatedAt = new DateTime(2026, 1, 1),
            UpdatedAt = new DateTime(2026, 1, 2)
        };

        documento.Should().BeAssignableTo<IAuditableEntity>();
        documento.AplicacionId.Should().Be(1);
        documento.NombreArchivo.Should().Be("Manual de Usuario");
        documento.UrlOneDrive.Should().Be("https://onedrive.example.com/manual");
        documento.Tipo.Should().Be("manual");
    }
}
