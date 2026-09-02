using AdminPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminPro.Infrastructure.Persistence.Configurations;

public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
{
    public void Configure(EntityTypeBuilder<Application> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Descripcion).HasMaxLength(500);
        builder.Property(a => a.TecnologiaFront).HasMaxLength(100);
        builder.Property(a => a.TecnologiaBack).HasMaxLength(100);
        builder.Property(a => a.RamaDesarrollo).HasMaxLength(100);
        builder.Property(a => a.ApplicationName).HasMaxLength(100);
        builder.Property(a => a.RutaLocal).HasMaxLength(500);
        builder.Property(a => a.RutaGit).HasMaxLength(500);
        builder.Property(a => a.ComoSeLevanta).HasMaxLength(500);

        builder.HasIndex(a => new { a.ProyectoId, a.Nombre })
            .IsUnique()
            .HasFilter("[Activo] = 1")
            .HasDatabaseName("IX_Applications_ProyectoId_Nombre");

        builder.HasQueryFilter(a => a.Activo);
    }
}
