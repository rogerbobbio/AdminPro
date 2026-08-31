using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AdminPro.Api.Tests.TestSupport;
using AdminPro.Application.Projects.Queries.GetProjectById;
using FluentAssertions;
using Xunit;

namespace AdminPro.Api.Tests;

public class BaseDeDatosControllerTests : IClassFixture<InMemoryApiFactory>
{
    private readonly HttpClient _client;

    public BaseDeDatosControllerTests(InMemoryApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<int> CreateProjectAsync(string nombre)
    {
        var response = await _client.PostAsJsonAsync("/api/projects", new { nombre });
        return await response.Content.ReadFromJsonAsync<int>();
    }

    [Fact]
    public async Task CreateDatabase_UnderProject_Returns201AndAppearsInDetail()
    {
        var projectId = await CreateProjectAsync("Acme Corp DB1");

        var createResponse = await _client.PostAsJsonAsync(
            $"/api/projects/{projectId}/basesdedatos",
            new { nombre = "SalesDb", ambiente = "desarrollo" });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var detail = await (await _client.GetAsync($"/api/projects/{projectId}")).Content.ReadFromJsonAsync<ProjectDetailDto>();
        detail!.BasesDeDatos.Should().ContainSingle(d => d.Nombre == "SalesDb");
    }

    [Fact]
    public async Task CreateDatabase_MissingProject_Returns404()
    {
        var response = await _client.PostAsJsonAsync("/api/projects/999999/basesdedatos", new { nombre = "SalesDb" });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateDatabase_ExistingDatabase_Returns204()
    {
        var projectId = await CreateProjectAsync("Acme Corp DB2");
        var createResponse = await _client.PostAsJsonAsync($"/api/projects/{projectId}/basesdedatos", new { nombre = "SalesDb" });
        var dbId = await createResponse.Content.ReadFromJsonAsync<int>();

        var updateResponse = await _client.PutAsJsonAsync($"/api/basesdedatos/{dbId}", new { id = dbId, nombre = "SalesDb", ambiente = "uat" });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var detail = await (await _client.GetAsync($"/api/projects/{projectId}")).Content.ReadFromJsonAsync<ProjectDetailDto>();
        detail!.BasesDeDatos.Single().Ambiente.Should().Be("uat");
    }

    [Fact]
    public async Task UpdateDatabase_MissingDatabase_Returns404()
    {
        var response = await _client.PutAsJsonAsync("/api/basesdedatos/999999", new { id = 999999, nombre = "Whatever" });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteDatabase_ExistingDatabase_Returns204AndRemovedFromDetail()
    {
        var projectId = await CreateProjectAsync("Acme Corp DB3");
        var createResponse = await _client.PostAsJsonAsync($"/api/projects/{projectId}/basesdedatos", new { nombre = "SalesDb" });
        var dbId = await createResponse.Content.ReadFromJsonAsync<int>();

        var deleteResponse = await _client.DeleteAsync($"/api/basesdedatos/{dbId}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var detail = await (await _client.GetAsync($"/api/projects/{projectId}")).Content.ReadFromJsonAsync<ProjectDetailDto>();
        detail!.BasesDeDatos.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteDatabase_MissingDatabase_Returns404()
    {
        var response = await _client.DeleteAsync("/api/basesdedatos/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
