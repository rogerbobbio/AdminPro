using AdminPro.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AdminPro.Api.Tests.TestSupport;

// Uses an isolated SQLite ":memory:" database (kept alive via an open connection for the
// factory's lifetime), so controller-level tests don't depend on the shared local/dev SQL
// Server instance. SQLite (not the EF Core InMemory provider) is required here because
// mutating endpoints run through TransactionBehavior, which calls
// Database.BeginTransactionAsync() - unsupported by the InMemory provider (it throws
// TransactionIgnoredWarning as an error) but supported by SQLite, matching the same
// workaround already used for TransactionBehavior's own unit tests in foundation-backend.
public class InMemoryApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    // A separate internal service provider for the Sqlite provider is required here:
    // Program.cs's real AddDbContext<AppDbContext>(UseSqlServer) already registered the
    // SqlServer provider's services in the app's DI container, so simply adding UseSqlite
    // on top leaves both providers registered side by side, which EF Core rejects as
    // ambiguous (same fix as the EF Core InMemory provider needed previously).
    private static readonly IServiceProvider SqliteProvider =
        new ServiceCollection().AddEntityFrameworkSqlite().BuildServiceProvider();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll(typeof(Microsoft.EntityFrameworkCore.Infrastructure.IDbContextOptionsConfiguration<AppDbContext>));
            services.AddDbContext<AppDbContext>(options => options
                .UseSqlite(_connection)
                .UseInternalServiceProvider(SqliteProvider));
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        // EnsureCreated() materializes the schema (and HasData seed rows, e.g. the two
        // seeded Modulos) directly from the model for this ad-hoc SQLite database - it
        // doesn't run migrations.
        using var scope = Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();

        base.ConfigureClient(client);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection.Dispose();
    }
}
