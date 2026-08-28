using AdminPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminPro.Infrastructure.Persistence.Configurations;

public class ServicioConfiguration : IEntityTypeConfiguration<Servicio>
{
    public void Configure(EntityTypeBuilder<Servicio> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(s => s.Tipo).HasMaxLength(50).IsRequired();
        builder.Property(s => s.Ambiente).HasMaxLength(50);
        builder.Property(s => s.Url).HasMaxLength(500).IsRequired();

        builder.HasOne(s => s.Project)
            .WithMany(p => p.Servicios)
            .HasForeignKey(s => s.ProyectoId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(s => new { s.ProyectoId, s.Tipo }).HasDatabaseName("IX_Servicios_ProyectoId_Tipo");
        builder.HasIndex(s => s.EsGlobal).HasDatabaseName("IX_Servicios_EsGlobal");

        builder.HasQueryFilter(s => s.Activo);
    }
}
