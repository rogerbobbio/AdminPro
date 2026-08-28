using AdminPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminPro.Infrastructure.Persistence.Configurations;

public class NotaConfiguration : IEntityTypeConfiguration<Nota>
{
    public void Configure(EntityTypeBuilder<Nota> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Titulo).HasMaxLength(200).IsRequired();
        builder.Property(n => n.Descripcion).IsRequired();

        builder.HasOne(n => n.Aplicacion)
            .WithMany(a => a.Notas)
            .HasForeignKey(n => n.AplicacionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(n => n.Activo);
    }
}
