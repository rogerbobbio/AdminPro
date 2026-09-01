using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AdminPro.Api.Tests.TestSupport;
using AdminPro.Application.Applications.Queries.GetApplicationById;
using FluentAssertions;
using Xunit;

namespace AdminPro.Api.Tests;

public class ApplicationsControllerTests : IClassFixture<InMemoryApiFactory>
{
    private readonly HttpClient _client;

    public ApplicationsControllerTests(InMemoryApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<int> CreateProjectAsync(string nombre)
    {
        var response = await _client.PostAsJsonAsync("/api/projects", new { nombre });
        return await response.Content.ReadFromJsonAsync<int>();
    }

    [Fact]
    public async Task CreateApplication_UnderProject_Returns201AndAppearsInList()
    {
        var projectId = await CreateProjectAsync("Acme Corp AC1");

        var createResponse = await _client.PostAsJsonAsync(
            $"/api/projects/{projectId}/applications",
            new { nombre = "CRM", tecnologiaFront = "Angular" });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var listResponse = await _client.GetAsync($"/api/projects/{projectId}/applications");
        listResponse.EnsureSuccessStatusCode();
        var list = await listResponse.Content.ReadFromJsonAsync<System.Collections.Generic.List<AdminPro.Application.Applications.Queries.GetApplicationsByProject.ApplicationSummaryDto>>();
        list.Should().ContainSingle(a => a.Nombre == "CRM");
    }

    [Fact]
    public async Task CreateApplication_DuplicateNameInSameProject_Returns400()
    {
        var projectId = await CreateProjectAsync("Acme Corp AC2");
        await _client.PostAsJsonAsync($"/api/projects/{projectId}/applications", new { nombre = "CRM" });

        var duplicateResponse = await _client.PostAsJsonAsync($"/api/projects/{projectId}/applications", new { nombre = "CRM" });

        duplicateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateApplication_MissingProject_Returns404()
    {
        var response = await _client.PostAsJsonAsync("/api/projects/999999/applications", new { nombre = "CRM" });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_ExistingApplication_ReturnsDetail()
    {
        var projectId = await CreateProjectAsync("Acme Corp AC3");
        var createResponse = await _client.PostAsJsonAsync($"/api/projects/{projectId}/applications", new { nombre = "CRM" });
        var id = await createResponse.Content.ReadFromJsonAsync<int>();

        var response = await _client.GetAsync($"/api/applications/{id}");

        response.EnsureSuccessStatusCode();
        var detail = await response.Content.ReadFromJsonAsync<ApplicationDetailDto>();
        detail!.Nombre.Should().Be("CRM");
        detail.Ambientes.Should().BeEmpty();
        detail.Reportes.Should().BeEmpty();
    }

    [Fact]
    public async Task GetById_MissingApplication_Returns404()
    {
        var response = await _client.GetAsync("/api/applications/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateApplication_ExistingApplication_Returns204()
    {
        var projectId = await CreateProjectAsync("Acme Corp AC4");
        var createResponse = await _client.PostAsJsonAsync($"/api/projects/{projectId}/applications", new { nombre = "CRM" });
        var id = await createResponse.Content.ReadFromJsonAsync<int>();

        var updateResponse = await _client.PutAsJsonAsync($"/api/applications/{id}", new { id, nombre = "CRM Updated" });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var detail = await (await _client.GetAsync($"/api/applications/{id}")).Content.ReadFromJsonAsync<ApplicationDetailDto>();
        detail!.Nombre.Should().Be("CRM Updated");
    }

    [Fact]
    public async Task UpdateApplication_DuplicateNameInSameProject_Returns400()
    {
        var projectId = await CreateProjectAsync("Acme Corp AC5");
        await _client.PostAsJsonAsync($"/api/projects/{projectId}/applications", new { nombre = "CRM" });
        var createResponse = await _client.PostAsJsonAsync($"/api/projects/{projectId}/applications", new { nombre = "Billing" });
        var id = await createResponse.Content.ReadFromJsonAsync<int>();

        var updateResponse = await _client.PutAsJsonAsync($"/api/applications/{id}", new { id, nombre = "CRM" });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateApplication_MissingApplication_Returns404()
    {
        var response = await _client.PutAsJsonAsync("/api/applications/999999", new { id = 999999, nombre = "Whatever" });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteApplication_ExistingApplication_Returns204AndCascadesToAmbientes()
    {
        var projectId = await CreateProjectAsync("Acme Corp AC6");
        var createResponse = await _client.PostAsJsonAsync($"/api/projects/{projectId}/applications", new { nombre = "CRM" });
        var id = await createResponse.Content.ReadFromJsonAsync<int>();
        await _client.PostAsJsonAsync($"/api/applications/{id}/ambientes", new { nombre = "UAT" });

        var deleteResponse = await _client.DeleteAsync($"/api/applications/{id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var getResponse = await _client.GetAsync($"/api/applications/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteApplication_MissingApplication_Returns404()
    {
        var response = await _client.DeleteAsync("/api/applications/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
