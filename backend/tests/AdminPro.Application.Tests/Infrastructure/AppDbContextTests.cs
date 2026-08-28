using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdminPro.Application.Tests.Infrastructure;

public class AppDbContextTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public void ExposesDbSetsForAllElevenEntities()
    {
        using var db = CreateInMemoryContext(nameof(ExposesDbSetsForAllElevenEntities));

        db.Modulos.Should().NotBeNull();
        db.Projects.Should().NotBeNull();
        db.BasesDeDatos.Should().NotBeNull();
        db.Applications.Should().NotBeNull();
        db.Ambientes.Should().NotBeNull();
        db.Reportes.Should().NotBeNull();
        db.Notas.Should().NotBeNull();
        db.Documentos.Should().NotBeNull();
        db.FixDatas.Should().NotBeNull();
        db.Servicios.Should().NotBeNull();
        db.AplicacionServicios.Should().NotBeNull();
    }
}
