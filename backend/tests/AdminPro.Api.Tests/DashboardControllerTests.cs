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
        summary.ApplicationsCreatedLast7Days.Should().HaveCount(7);
        summary.RecentApplications.Should().BeEmpty();
    }
}
