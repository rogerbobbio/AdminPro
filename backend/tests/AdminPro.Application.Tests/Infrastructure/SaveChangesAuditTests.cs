using System;
using System.Threading.Tasks;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdminPro.Application.Tests.Infrastructure;

public class SaveChangesAuditTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task SaveChangesAsync_SetsUpdatedAt_OnModifiedEntities()
    {
        using var db = CreateInMemoryContext(nameof(SaveChangesAsync_SetsUpdatedAt_OnModifiedEntities));
        var originalTimestamp = new DateTime(2020, 1, 1);
        var project = new Project { Nombre = "Acme Corp", CreatedAt = originalTimestamp, UpdatedAt = originalTimestamp };
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        project.Nombre = "Acme Corp Updated";
        await db.SaveChangesAsync();

        project.CreatedAt.Should().Be(originalTimestamp);
        project.UpdatedAt.Should().BeAfter(originalTimestamp);
        project.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task SaveChangesAsync_DoesNotTouchUpdatedAt_OnNewlyAddedEntities()
    {
        using var db = CreateInMemoryContext(nameof(SaveChangesAsync_DoesNotTouchUpdatedAt_OnNewlyAddedEntities));
        var explicitTimestamp = new DateTime(2020, 1, 1);
        var project = new Project { Nombre = "Acme Corp", CreatedAt = explicitTimestamp, UpdatedAt = explicitTimestamp };

        db.Projects.Add(project);
        await db.SaveChangesAsync();

        project.UpdatedAt.Should().Be(explicitTimestamp);
    }
}
