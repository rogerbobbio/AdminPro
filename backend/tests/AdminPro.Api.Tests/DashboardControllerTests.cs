using System.Net.Http.Json;
using System.Threading.Tasks;
using AdminPro.Api.Tests.TestSupport;
using AdminPro.Application.Dashboard.Queries.GetDashboardSummary;
using FluentAssertions;
using Xunit;

namespace AdminPro.Api.Tests;

public class DashboardControllerTests : IClassFixture<InMemoryApiFactory>
{
    private readonly HttpClient _client;

    public DashboardControllerTests(InMemoryApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetSummary_EmptyDatabase_ReturnsAllZeroShape()
    {
        var response = await _client.GetAsync("/api/dashboard/summary");

        response.EnsureSuccessStatusCode();
        var summary = await response.Content.ReadFromJsonAsync<DashboardSummaryDto>();

        summary.Should().NotBeNull();
        summary!.TotalProjects.Should().Be(0);
        summary.TotalApplications.Should().Be(0);
        summary.RecentApplications.Should().BeEmpty();
    }
}

// Separate class so it gets its own InMemoryApiFactory instance (a fresh SQLite ":memory:"
// database) rather than sharing DashboardControllerTests' fixture, which must stay empty
// for its own zero-state assertions.
public class DashboardSummaryOrderingTests : IClassFixture<InMemoryApiFactory>
{
    private readonly HttpClient _client;

    public DashboardSummaryOrderingTests(InMemoryApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<int> CreateProjectAsync(string nombre)
    {
        var response = await _client.PostAsJsonAsync("/api/projects", new { nombre });
        return await response.Content.ReadFromJsonAsync<int>();
    }

    [Fact]
    public async Task GetSummary_RecentApplications_OrderedByUpdatedAtNotCreatedAt()
    {
        var projectId = await CreateProjectAsync("Dashboard Order Co");
        var olderCreateResponse = await _client.PostAsJsonAsync(
            $"/api/projects/{projectId}/applications",
            new { nombre = "Created First" });
        var olderId = await olderCreateResponse.Content.ReadFromJsonAsync<int>();
        await Task.Delay(10);
        var newerCreateResponse = await _client.PostAsJsonAsync(
            $"/api/projects/{projectId}/applications",
            new { nombre = "Created Second" });
        var newerId = await newerCreateResponse.Content.ReadFromJsonAsync<int>();
        await Task.Delay(10);

        // Touch the OLDER application last, so it becomes the most recently *modified*
        // despite being created first.
        await _client.PutAsJsonAsync($"/api/applications/{olderId}", new { id = olderId, nombre = "Created First" });

        var response = await _client.GetAsync("/api/dashboard/summary");

        response.EnsureSuccessStatusCode();
        var summary = await response.Content.ReadFromJsonAsync<DashboardSummaryDto>();

        summary!.RecentApplications.Should().HaveCountGreaterThanOrEqualTo(2);
        var first = summary.RecentApplications[0];
        var second = summary.RecentApplications[1];
        first.Id.Should().Be(olderId);
        second.Id.Should().Be(newerId);
        first.UpdatedAt.Should().BeAfter(second.UpdatedAt);
    }
}
