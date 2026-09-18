using System;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Dashboard.Queries.GetDashboardSummary;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AppEntity = AdminPro.Domain.Entities.Application;

namespace AdminPro.Application.Tests.Dashboard;

public class GetDashboardSummaryQueryHandlerTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handle_EmptyDatabase_ReturnsAllZeros()
    {
        using var db = CreateInMemoryContext(nameof(Handle_EmptyDatabase_ReturnsAllZeros));
        var handler = new GetDashboardSummaryQueryHandler(db);

        var result = await handler.Handle(new GetDashboardSummaryQuery(), CancellationToken.None);

        result.TotalProjects.Should().Be(0);
        result.TotalApplications.Should().Be(0);
        result.RecentApplications.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_CountsOnlyActiveRows()
    {
        using var db = CreateInMemoryContext(nameof(Handle_CountsOnlyActiveRows));
        var project = new Project { Nombre = "Acme", Activo = true };
        var inactiveProject = new Project { Nombre = "Inactive Co", Activo = false };
        db.Projects.AddRange(project, inactiveProject);
        await db.SaveChangesAsync();

        db.Applications.AddRange(
            new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new AppEntity { ProyectoId = project.Id, Nombre = "ERP", Activo = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new AppEntity { ProyectoId = project.Id, Nombre = "Legacy", Activo = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var handler = new GetDashboardSummaryQueryHandler(db);
        var result = await handler.Handle(new GetDashboardSummaryQuery(), CancellationToken.None);

        result.TotalProjects.Should().Be(1);
        result.TotalApplications.Should().Be(2);
    }

    [Fact]
    public async Task Handle_RecentApplications_ReturnsAtMostFive_OrderedByUpdatedAtDescending()
    {
        using var db = CreateInMemoryContext(nameof(Handle_RecentApplications_ReturnsAtMostFive_OrderedByUpdatedAtDescending));
        var project = new Project { Nombre = "Acme", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        for (var i = 0; i < 7; i++)
        {
            var timestamp = DateTime.UtcNow.AddMinutes(i);
            db.Applications.Add(new AppEntity
            {
                ProyectoId = project.Id,
                Nombre = $"App{i}",
                TecnologiaFront = "Angular 18",
                Activo = true,
                CreatedAt = timestamp,
                UpdatedAt = timestamp,
            });
        }

        await db.SaveChangesAsync();

        var handler = new GetDashboardSummaryQueryHandler(db);
        var result = await handler.Handle(new GetDashboardSummaryQuery(), CancellationToken.None);

        result.RecentApplications.Should().HaveCount(5);
        result.RecentApplications[0].Nombre.Should().Be("App6");
        result.RecentApplications[0].ProjectName.Should().Be("Acme");
    }

    [Fact]
    public async Task Handle_RecentApplications_ReflectsUpdatedAtNotCreatedAt()
    {
        using var db = CreateInMemoryContext(nameof(Handle_RecentApplications_ReflectsUpdatedAtNotCreatedAt));
        var project = new Project { Nombre = "Acme", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        var createdFirstButUpdatedLast = new AppEntity
        {
            ProyectoId = project.Id,
            Nombre = "Created First",
            Activo = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow.AddMinutes(10),
        };
        var createdSecondButNeverUpdated = new AppEntity
        {
            ProyectoId = project.Id,
            Nombre = "Created Second",
            Activo = true,
            CreatedAt = DateTime.UtcNow.AddMinutes(1),
            UpdatedAt = DateTime.UtcNow.AddMinutes(1),
        };
        db.Applications.AddRange(createdFirstButUpdatedLast, createdSecondButNeverUpdated);
        await db.SaveChangesAsync();

        var handler = new GetDashboardSummaryQueryHandler(db);
        var result = await handler.Handle(new GetDashboardSummaryQuery(), CancellationToken.None);

        result.RecentApplications[0].Nombre.Should().Be("Created First");
        result.RecentApplications[0].UpdatedAt.Should().Be(createdFirstButUpdatedLast.UpdatedAt);
    }
}
