using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Modulos.Queries.GetModulos;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdminPro.Application.Tests.Modulos;

public class GetModulosQueryHandlerTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handle_ReturnsOnlyActiveModules_OrderedByOrden()
    {
        using var db = CreateInMemoryContext(nameof(Handle_ReturnsOnlyActiveModules_OrderedByOrden));
        db.Modulos.AddRange(
            new Modulo { Nombre = "Gestión de Proyectos", RutaBase = "proyectos", Orden = 0, Activo = true },
            new Modulo { Nombre = "Presupuesto (inactivo)", RutaBase = "presupuesto", Orden = 1, Activo = false },
            new Modulo { Nombre = "Catálogo de Servicios", RutaBase = "servicios", Orden = 2, Activo = true });
        await db.SaveChangesAsync();

        var handler = new GetModulosQueryHandler(db);
        var result = await handler.Handle(new GetModulosQuery(), CancellationToken.None);

        result.Select(m => m.Nombre).Should().Equal("Gestión de Proyectos", "Catálogo de Servicios");
        result.Select(m => m.Orden).Should().BeInAscendingOrder();
    }
}
