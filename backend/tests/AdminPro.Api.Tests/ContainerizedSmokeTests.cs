using AdminPro.Api.Tests.TestSupport;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AdminPro.Api.Tests;

public class ContainerizedSmokeTests : IClassFixture<ContainerizedApiFactory>
{
    private readonly ContainerizedApiFactory _factory;

    public ContainerizedSmokeTests(ContainerizedApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Api_BootsAgainstContainerizedSqlServer_WithEmptySchema()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        (await db.Database.CanConnectAsync()).Should().BeTrue();

        // Modulos is no longer empty post-migration: the SeedModulos migration
        // (frontend-dashboard change) inserts the 2 real modules per rule MOD-004.
        (await db.Modulos.IgnoreQueryFilters().CountAsync()).Should().Be(2);
        (await db.Projects.IgnoreQueryFilters().CountAsync()).Should().Be(0);
        (await db.Applications.IgnoreQueryFilters().CountAsync()).Should().Be(0);
    }
}
