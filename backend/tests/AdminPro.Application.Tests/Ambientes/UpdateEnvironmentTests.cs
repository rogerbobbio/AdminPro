using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Ambientes.Commands.UpdateEnvironment;
using AdminPro.Application.Common.Exceptions;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AppEntity = AdminPro.Domain.Entities.Application;

namespace AdminPro.Application.Tests.Ambientes;

public class UpdateEnvironmentTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"{nameof(UpdateEnvironmentTests)}.{dbName}")
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handler_UpdatesFields()
    {
        using var db = CreateInMemoryContext(nameof(Handler_UpdatesFields));
        var project = new Project { Nombre = "Acme Corp", Activo = true };
        db.Projects.Add(project);
        await db.SaveChangesAsync();
        var application = new AppEntity { ProyectoId = project.Id, Nombre = "CRM", Activo = true };
        db.Applications.Add(application);
        await db.SaveChangesAsync();
        var ambiente = new Ambiente { AplicacionId = application.Id, Nombre = "UAT", Activo = true };
        db.Ambientes.Add(ambiente);
        await db.SaveChangesAsync();

        var handler = new UpdateEnvironmentCommandHandler(db);
        await handler.Handle(
            new UpdateEnvironmentCommand(ambiente.Id, "UAT", "https://uat2.example.com", true, "Requiere VPN", 1),
            CancellationToken.None);

        var updated = await db.Ambientes.FindAsync(ambiente.Id);
        updated!.Url.Should().Be("https://uat2.example.com");
        updated.EsWebApi.Should().BeTrue();
        updated.Notas.Should().Be("Requiere VPN");
    }

    [Fact]
    public async Task Handler_MissingEnvironment_ThrowsNotFoundException()
    {
        using var db = CreateInMemoryContext(nameof(Handler_MissingEnvironment_ThrowsNotFoundException));
        var handler = new UpdateEnvironmentCommandHandler(db);

        var act = async () => await handler.Handle(
            new UpdateEnvironmentCommand(999, "UAT", null, false, null, 0),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
