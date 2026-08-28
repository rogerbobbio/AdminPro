using AdminPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminPro.Infrastructure.Persistence.Configurations;

public class AmbienteConfiguration : IEntityTypeConfiguration<Ambiente>
{
    public void Configure(EntityTypeBuilder<Ambiente> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Nombre).HasMaxLength(50).IsRequired();
        builder.Property(a => a.Url).HasMaxLength(500);

        builder.HasOne(a => a.Aplicacion)
            .WithMany(app => app.Ambientes)
            .HasForeignKey(a => a.AplicacionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => new { a.AplicacionId, a.Orden }).HasDatabaseName("IX_Ambientes_AplicacionId_Orden");

        builder.HasQueryFilter(a => a.Activo);
    }
}
