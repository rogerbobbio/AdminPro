using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AdminPro.Api.Tests.TestSupport;
using AdminPro.Application.Projects.Queries.GetProjectById;
using AdminPro.Application.Projects.Queries.GetProjects;
using FluentAssertions;
using Xunit;

namespace AdminPro.Api.Tests;

public class ProjectsControllerTests : IClassFixture<InMemoryApiFactory>
{
    private readonly HttpClient _client;

    public ProjectsControllerTests(InMemoryApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateProject_ThenGetAll_ContainsIt()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/projects", new { nombre = "Acme Corp PC1", descripcion = "Sistema" });
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var newId = await createResponse.Content.ReadFromJsonAsync<int>();
        newId.Should().BeGreaterThan(0);

        var listResponse = await _client.GetAsync("/api/projects");
        listResponse.EnsureSuccessStatusCode();
        var projects = await listResponse.Content.ReadFromJsonAsync<List<ProjectSummaryDto>>();
        projects.Should().Contain(p => p.Nombre == "Acme Corp PC1");
    }

    [Fact]
    public async Task CreateProject_DuplicateName_Returns400()
    {
        await _client.PostAsJsonAsync("/api/projects", new { nombre = "Acme Corp PC2" });

        var duplicateResponse = await _client.PostAsJsonAsync("/api/projects", new { nombre = "Acme Corp PC2" });

        duplicateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_MissingProject_Returns404()
    {
        var response = await _client.GetAsync("/api/projects/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_ExistingProject_ReturnsDetail()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/projects", new { nombre = "Acme Corp PC3" });
        var id = await createResponse.Content.ReadFromJsonAsync<int>();

        var response = await _client.GetAsync($"/api/projects/{id}");

        response.EnsureSuccessStatusCode();
        var detail = await response.Content.ReadFromJsonAsync<ProjectDetailDto>();
        detail!.Nombre.Should().Be("Acme Corp PC3");
        detail.BasesDeDatos.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateProject_ExistingProject_Returns204()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/projects", new { nombre = "Acme Corp PC4" });
        var id = await createResponse.Content.ReadFromJsonAsync<int>();

        var updateResponse = await _client.PutAsJsonAsync($"/api/projects/{id}", new { id, nombre = "Acme Corp PC4 Updated" });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var detail = await (await _client.GetAsync($"/api/projects/{id}")).Content.ReadFromJsonAsync<ProjectDetailDto>();
        detail!.Nombre.Should().Be("Acme Corp PC4 Updated");
    }

    [Fact]
    public async Task UpdateProject_MissingProject_Returns404()
    {
        var response = await _client.PutAsJsonAsync("/api/projects/999999", new { id = 999999, nombre = "Whatever" });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteProject_ExistingProject_Returns204AndCascades()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/projects", new { nombre = "Acme Corp PC5" });
        var id = await createResponse.Content.ReadFromJsonAsync<int>();
        await _client.PostAsJsonAsync($"/api/projects/{id}/basesdedatos", new { nombre = "SalesDb" });

        var deleteResponse = await _client.DeleteAsync($"/api/projects/{id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var getResponse = await _client.GetAsync($"/api/projects/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteProject_MissingProject_Returns404()
    {
        var response = await _client.DeleteAsync("/api/projects/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
