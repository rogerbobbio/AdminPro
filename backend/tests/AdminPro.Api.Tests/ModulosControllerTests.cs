using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AdminPro.Api.Tests.TestSupport;
using AdminPro.Application.Modulos.Queries.GetModulos;
using FluentAssertions;
using Xunit;

namespace AdminPro.Api.Tests;

public class ModulosControllerTests : IClassFixture<InMemoryApiFactory>
{
    private readonly HttpClient _client;

    public ModulosControllerTests(InMemoryApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSeededModulesInOrder()
    {
        var response = await _client.GetAsync("/api/modulos");

        response.EnsureSuccessStatusCode();
        var modulos = await response.Content.ReadFromJsonAsync<List<ModuloDto>>();

        modulos.Should().NotBeNull();
        modulos!.Select(m => m.Nombre).Should().Equal("Gestión de Proyectos", "Catálogo de Servicios");
    }
}
