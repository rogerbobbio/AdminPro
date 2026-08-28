using AdminPro.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Testcontainers.MsSql;
using Xunit;

namespace AdminPro.Api.Tests.TestSupport;

public class ContainerizedApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlBuilder _containerBuilder = new("mcr.microsoft.com/mssql/server:2022-latest");
    private MsSqlContainer _sqlContainer = null!;

    public CapturingLoggerProvider LogCapture { get; } = new();

    public async Task InitializeAsync()
    {
        _sqlContainer = _containerBuilder.Build();
        await _sqlContainer.StartAsync();

        // Force host creation now so ConfigureWebHost runs against the started container.
        _ = Server;

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging => logging.AddProvider(LogCapture));

        builder.ConfigureServices(services =>
        {
            services.AddControllers().AddApplicationPart(typeof(TestOnlyController).Assembly);

            // Test-only command/handler/validator for the pipeline-order integration test.
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(TestOnlyController).Assembly));
            services.AddValidatorsFromAssembly(typeof(TestOnlyController).Assembly);

            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(_sqlContainer.GetConnectionString()));
        });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _sqlContainer.DisposeAsync();
    }
}
