using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AdminPro.Api.Tests.TestSupport;
using AdminPro.Application.Applications.Queries.GetApplicationById;
using FluentAssertions;
using Xunit;

namespace AdminPro.Api.Tests;

// Full-stack integration: real SQL Server (via Testcontainers), real FK constraints,
// real transactions - confirms the create -> get -> update -> delete -> cascade flow for
// both Application and Ambiente against actual relational behavior, mirroring
// ProjectsIntegrationTests' pattern for Project/BaseDeDatos.
public class ApplicationsIntegrationTests : IClassFixture<ContainerizedApiFactory>
{
    private readonly ContainerizedApiFactory _factory;

    public ApplicationsIntegrationTests(ContainerizedApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task FullCrudFlow_ForApplicationAndAmbiente_WorksAgainstRealSqlServer()
    {
        var client = _factory.CreateClient();

        var createProjectResponse = await client.PostAsJsonAsync("/api/projects", new { nombre = "Acme Corp App Integration" });
        createProjectResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var projectId = await createProjectResponse.Content.ReadFromJsonAsync<int>();

        var createAppResponse = await client.PostAsJsonAsync(
            $"/api/projects/{projectId}/applications", new { nombre = "CRM", tecnologiaFront = "Angular" });
        createAppResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var appId = await createAppResponse.Content.ReadFromJsonAsync<int>();

        var createEnvResponse = await client.PostAsJsonAsync(
            $"/api/applications/{appId}/ambientes", new { nombre = "UAT", url = "https://uat.example.com" });
        createEnvResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var envId = await createEnvResponse.Content.ReadFromJsonAsync<int>();

        var updateResponse = await client.PutAsJsonAsync(
            $"/api/applications/{appId}", new { id = appId, nombre = "CRM Updated" });
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var detailResponse = await client.GetAsync($"/api/applications/{appId}");
        var detail = await detailResponse.Content.ReadFromJsonAsync<ApplicationDetailDto>();
        detail!.Nombre.Should().Be("CRM Updated");
        detail.Ambientes.Should().ContainSingle(e => e.Id == envId && e.Nombre == "UAT");

        var deleteResponse = await client.DeleteAsync($"/api/applications/{appId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var afterDeleteResponse = await client.GetAsync($"/api/applications/{appId}");
        afterDeleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
