using System;
using AdminPro.Domain.Entities;
using AdminPro.Domain.Interfaces;
using FluentAssertions;
using Xunit;

namespace AdminPro.Application.Tests.Domain;

public class ReporteTests
{
    [Fact]
    public void SatisfiesAuditableEntityAndOwnProperties()
    {
        var reporte = new Reporte
        {
            Id = 1,
            AplicacionId = 1,
            ReportCode = "VFL",
            ReportName = "Volumen de Carga",
            RegionId = "DLA",
            ReportPath = "/volume-for-load",
            SpTranship = "sp_Tranship",
            SpReportViewer = "sp_ReportViewer",
            Notas = "nota",
            ParametrosEjemplo = "{}",
            Activo = true,
            CreatedAt = new DateTime(2026, 1, 1),
            UpdatedAt = new DateTime(2026, 1, 2)
        };

        reporte.Should().BeAssignableTo<IAuditableEntity>();
        reporte.AplicacionId.Should().Be(1);
        reporte.ReportCode.Should().Be("VFL");
        reporte.ReportName.Should().Be("Volumen de Carga");
    }
}
