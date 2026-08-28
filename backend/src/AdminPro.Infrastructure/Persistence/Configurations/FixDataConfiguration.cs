using AdminPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminPro.Infrastructure.Persistence.Configurations;

public class FixDataConfiguration : IEntityTypeConfiguration<FixData>
{
    public void Configure(EntityTypeBuilder<FixData> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(f => f.Descripcion).HasMaxLength(500);

        builder.HasOne(f => f.Aplicacion)
            .WithMany(a => a.FixDatas)
            .HasForeignKey(f => f.AplicacionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(f => f.Activo);
    }
}
