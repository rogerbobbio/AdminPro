using AdminPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminPro.Infrastructure.Persistence.Configurations;

public class DocumentoConfiguration : IEntityTypeConfiguration<Documento>
{
    public void Configure(EntityTypeBuilder<Documento> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.NombreArchivo).HasMaxLength(200).IsRequired();
        builder.Property(d => d.UrlOneDrive).HasMaxLength(500).IsRequired();
        builder.Property(d => d.Tipo).HasMaxLength(50).IsRequired();
        builder.Property(d => d.Descripcion).HasMaxLength(500);

        builder.HasOne(d => d.Aplicacion)
            .WithMany(a => a.Documentos)
            .HasForeignKey(d => d.AplicacionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(d => d.Activo);
    }
}
