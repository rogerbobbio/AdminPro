using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AdminPro.Api.Tests.TestSupport;
using AdminPro.Application.Applications.Queries.GetApplicationById;
using FluentAssertions;
using Xunit;

namespace AdminPro.Api.Tests;

public class AmbientesControllerTests : IClassFixture<InMemoryApiFactory>
{
    private readonly HttpClient _client;

    public AmbientesControllerTests(InMemoryApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<int> CreateApplicationAsync(string suffix)
    {
        var projectResponse = await _client.PostAsJsonAsync("/api/projects", new { nombre = $"Acme Corp {suffix}" });
        var projectId = await projectResponse.Content.ReadFromJsonAsync<int>();
        var appResponse = await _client.PostAsJsonAsync($"/api/projects/{projectId}/applications", new { nombre = "CRM" });
        return await appResponse.Content.ReadFromJsonAsync<int>();
    }

    [Fact]
    public async Task CreateEnvironment_UnderApplication_Returns201AndAppearsInDetail()
    {
        var appId = await CreateApplicationAsync("EN1");

        var createResponse = await _client.PostAsJsonAsync(
            $"/api/applications/{appId}/ambientes",
            new { nombre = "UAT", url = "https://uat.example.com" });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var detail = await (await _client.GetAsync($"/api/applications/{appId}")).Content.ReadFromJsonAsync<ApplicationDetailDto>();
        detail!.Ambientes.Should().ContainSingle(e => e.Nombre == "UAT");
    }

    [Fact]
    public async Task CreateEnvironment_InvalidUrl_Returns400()
    {
        var appId = await CreateApplicationAsync("EN2");

        var response = await _client.PostAsJsonAsync(
            $"/api/applications/{appId}/ambientes",
            new { nombre = "UAT", url = "not-a-url" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateEnvironment_MissingApplication_Returns404()
    {
        var response = await _client.PostAsJsonAsync("/api/applications/999999/ambientes", new { nombre = "UAT" });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateEnvironment_ExistingEnvironment_Returns204()
    {
        var appId = await CreateApplicationAsync("EN3");
        var createResponse = await _client.PostAsJsonAsync($"/api/applications/{appId}/ambientes", new { nombre = "UAT" });
        var envId = await createResponse.Content.ReadFromJsonAsync<int>();

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/ambientes/{envId}", new { id = envId, nombre = "UAT", url = "https://uat2.example.com" });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var detail = await (await _client.GetAsync($"/api/applications/{appId}")).Content.ReadFromJsonAsync<ApplicationDetailDto>();
        detail!.Ambientes.Single().Url.Should().Be("https://uat2.example.com");
    }

    [Fact]
    public async Task UpdateEnvironment_MissingEnvironment_Returns404()
    {
        var response = await _client.PutAsJsonAsync("/api/ambientes/999999", new { id = 999999, nombre = "Whatever" });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteEnvironment_ExistingEnvironment_Returns204AndRemovedFromDetail()
    {
        var appId = await CreateApplicationAsync("EN4");
        var createResponse = await _client.PostAsJsonAsync($"/api/applications/{appId}/ambientes", new { nombre = "UAT" });
        var envId = await createResponse.Content.ReadFromJsonAsync<int>();

        var deleteResponse = await _client.DeleteAsync($"/api/ambientes/{envId}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var detail = await (await _client.GetAsync($"/api/applications/{appId}")).Content.ReadFromJsonAsync<ApplicationDetailDto>();
        detail!.Ambientes.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteEnvironment_MissingEnvironment_Returns404()
    {
        var response = await _client.DeleteAsync("/api/ambientes/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
