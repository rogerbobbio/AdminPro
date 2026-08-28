using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AdminPro.Api.Tests.TestSupport;
using FluentAssertions;
using Xunit;

namespace AdminPro.Api.Tests;

public class ExceptionHandlerMiddlewareTests : IClassFixture<TestingWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ExceptionHandlerMiddlewareTests(TestingWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ValidationException_MapsTo400WithStructuredBody()
    {
        var response = await _client.GetAsync("/api/testonly/validation-error");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        json.GetProperty("error").GetString().Should().Be("ValidationError");
        json.GetProperty("details")[0].GetProperty("field").GetString().Should().Be("Nombre");
    }

    [Fact]
    public async Task UnhandledException_MapsTo500WithStructuredBody()
    {
        var response = await _client.GetAsync("/api/testonly/unhandled-error");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        json.GetProperty("error").GetString().Should().Be("InternalServerError");
    }
}
