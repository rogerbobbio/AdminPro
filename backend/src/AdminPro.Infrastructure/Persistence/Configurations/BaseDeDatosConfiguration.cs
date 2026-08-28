using AdminPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminPro.Infrastructure.Persistence.Configurations;

public class BaseDeDatosConfiguration : IEntityTypeConfiguration<BaseDeDatos>
{
    public void Configure(EntityTypeBuilder<BaseDeDatos> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(d => d.Servidor).HasMaxLength(200);
        builder.Property(d => d.LoginName).HasMaxLength(100);
        builder.Property(d => d.Ambiente).HasMaxLength(50);

        builder.HasQueryFilter(d => d.Activo);
    }
}
