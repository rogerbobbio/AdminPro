using AdminPro.Domain.Entities;
using AdminPro.Domain.Interfaces;
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

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
