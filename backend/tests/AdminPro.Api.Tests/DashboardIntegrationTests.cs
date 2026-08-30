using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AdminPro.Api.Tests.TestSupport;
using AdminPro.Application.Dashboard.Queries.GetDashboardSummary;
using AdminPro.Application.Modulos.Queries.GetModulos;
using FluentAssertions;
using Xunit;

namespace AdminPro.Api.Tests;

// Full-stack integration: real SQL Server (via Testcontainers), real migrations
// (including SeedModulos), real HTTP round trip.
public class DashboardIntegrationTests : IClassFixture<ContainerizedApiFactory>
{
    private readonly ContainerizedApiFactory _factory;

    public DashboardIntegrationTests(ContainerizedApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetModulos_ReturnsSeededModulesAfterRealMigration()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/modulos");

        response.EnsureSuccessStatusCode();
        var modulos = await response.Content.ReadFromJsonAsync<List<ModuloDto>>();

        modulos.Should().NotBeNull();
        modulos!.Select(m => m.Nombre).Should().Equal("Gestión de Proyectos", "Catálogo de Servicios");
    }

    [Fact]
    public async Task GetDashboardSummary_ReturnsRealAggregateShape()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/dashboard/summary");

        response.EnsureSuccessStatusCode();
        var summary = await response.Content.ReadFromJsonAsync<DashboardSummaryDto>();

        summary.Should().NotBeNull();
        summary!.ApplicationsCreatedLast7Days.Should().HaveCount(7);
    }
}
