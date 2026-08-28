using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdminPro.Application.Tests.Infrastructure;

public class SoftDeleteFilterTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task InactiveRow_IsExcludedByDefault()
    {
        using var db = CreateInMemoryContext(nameof(InactiveRow_IsExcludedByDefault));
        db.Projects.Add(new Project { Nombre = "Acme Corp", Activo = false });
        await db.SaveChangesAsync();

        var result = await db.Projects.ToListAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task InactiveRow_IsVisibleWithIgnoreQueryFilters()
    {
        using var db = CreateInMemoryContext(nameof(InactiveRow_IsVisibleWithIgnoreQueryFilters));
        db.Projects.Add(new Project { Nombre = "Acme Corp", Activo = false });
        await db.SaveChangesAsync();

        var result = await db.Projects.IgnoreQueryFilters().ToListAsync();

        result.Should().ContainSingle(p => p.Nombre == "Acme Corp");
    }
}
