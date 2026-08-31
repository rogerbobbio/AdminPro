using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AdminPro.Api.Tests.TestSupport;
using AdminPro.Application.Projects.Queries.GetProjectById;
using FluentAssertions;
using Xunit;

namespace AdminPro.Api.Tests;

// Full-stack integration: real SQL Server (via Testcontainers), real FK constraints,
// real transactions - confirms the create -> get -> update -> delete -> cascade flow for
// both Project and BaseDeDatos against actual relational behavior, not just the InMemory
// or SQLite fixtures used by the faster controller-level tests.
public class ProjectsIntegrationTests : IClassFixture<ContainerizedApiFactory>
{
    private readonly ContainerizedApiFactory _factory;

    public ProjectsIntegrationTests(ContainerizedApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task FullCrudFlow_ForProjectAndDatabase_WorksAgainstRealSqlServer()
    {
        var client = _factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/projects", new { nombre = "Acme Corp Integration" });
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var projectId = await createResponse.Content.ReadFromJsonAsync<int>();

        var createDbResponse = await client.PostAsJsonAsync(
            $"/api/projects/{projectId}/basesdedatos", new { nombre = "SalesDb", ambiente = "desarrollo" });
        createDbResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var dbId = await createDbResponse.Content.ReadFromJsonAsync<int>();

        var updateResponse = await client.PutAsJsonAsync(
            $"/api/projects/{projectId}", new { id = projectId, nombre = "Acme Corp Integration Updated" });
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var detailResponse = await client.GetAsync($"/api/projects/{projectId}");
        var detail = await detailResponse.Content.ReadFromJsonAsync<ProjectDetailDto>();
        detail!.Nombre.Should().Be("Acme Corp Integration Updated");
        detail.BasesDeDatos.Should().ContainSingle(d => d.Id == dbId && d.Nombre == "SalesDb");

        var deleteResponse = await client.DeleteAsync($"/api/projects/{projectId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var afterDeleteResponse = await client.GetAsync($"/api/projects/{projectId}");
        afterDeleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
