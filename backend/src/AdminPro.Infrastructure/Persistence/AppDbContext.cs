using AdminPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Modulo> Modulos => Set<Modulo>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<BaseDeDatos> BasesDeDatos => Set<BaseDeDatos>();
    public DbSet<Application> Applications => Set<Application>();
    public DbSet<Ambiente> Ambientes => Set<Ambiente>();
    public DbSet<Reporte> Reportes => Set<Reporte>();
    public DbSet<Nota> Notas => Set<Nota>();
    public DbSet<Documento> Documentos => Set<Documento>();
    public DbSet<FixData> FixDatas => Set<FixData>();
    public DbSet<Servicio> Servicios => Set<Servicio>();
    public DbSet<AplicacionServicio> AplicacionServicios => Set<AplicacionServicio>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
