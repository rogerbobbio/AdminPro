using AdminPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminPro.Infrastructure.Persistence.Configurations;

public class ModuloConfiguration : IEntityTypeConfiguration<Modulo>
{
    public void Configure(EntityTypeBuilder<Modulo> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(m => m.Icono).HasMaxLength(50);
        builder.Property(m => m.RutaBase).HasMaxLength(50).IsRequired();
        builder.Property(m => m.Color).HasMaxLength(20);

        builder.HasIndex(m => m.Nombre).IsUnique().HasFilter("[Activo] = 1").HasDatabaseName("IX_Modulos_Nombre");
        builder.HasIndex(m => m.RutaBase).IsUnique().HasFilter("[Activo] = 1").HasDatabaseName("IX_Modulos_RutaBase");

        builder.HasQueryFilter(m => m.Activo);
    }
}
