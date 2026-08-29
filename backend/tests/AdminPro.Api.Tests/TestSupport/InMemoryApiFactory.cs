using AdminPro.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AdminPro.Api.Tests.TestSupport;

// Swaps AppDbContext for an isolated in-memory database, so controller-level tests
// don't depend on the shared local/dev SQL Server instance. Full SQL Server behavior
// (migrations, seed data, real transactions) is covered separately by ContainerizedApiFactory.
public class InMemoryApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    // A separate internal service provider for the InMemory provider is required here:
    // Program.cs's real AddDbContext<AppDbContext>(UseSqlServer) already registered the
    // SqlServer provider's services in the app's DI container, so simply adding
    // UseInMemoryDatabase on top leaves both providers registered side by side, which EF
    // Core rejects as ambiguous.
    private static readonly IServiceProvider InMemoryProvider =
        new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options => options
                .UseInMemoryDatabase(_databaseName)
                .UseInternalServiceProvider(InMemoryProvider));
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        // EnsureCreated() is what actually materializes HasData seed rows (e.g. the two
        // seeded Modulos) for the InMemory provider - it doesn't run migrations. Runs here
        // (once the real host/service provider exists) rather than in ConfigureWebHost,
        // where building a second provider from the raw ServiceCollection would register
        // the SqlServer and InMemory providers side by side and EF Core rejects that.
        using var scope = Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();

        base.ConfigureClient(client);
    }
}
