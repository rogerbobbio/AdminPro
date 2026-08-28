using AdminPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminPro.Infrastructure.Persistence.Configurations;

public class AplicacionServicioConfiguration : IEntityTypeConfiguration<AplicacionServicio>
{
    public void Configure(EntityTypeBuilder<AplicacionServicio> builder)
    {
        builder.HasKey(x => new { x.AplicacionId, x.ServicioId });

        builder.Property(x => x.NotasEspecificas).HasMaxLength(500);

        builder.HasOne(x => x.Aplicacion)
            .WithMany(a => a.AplicacionServicios)
            .HasForeignKey(x => x.AplicacionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Servicio)
            .WithMany(s => s.AplicacionServicios)
            .HasForeignKey(x => x.ServicioId)
            .OnDelete(DeleteBehavior.Cascade);

        // No HasQueryFilter: AplicacionServicio is a pure link entity, not IAuditableEntity (no Activo column).
    }
}
