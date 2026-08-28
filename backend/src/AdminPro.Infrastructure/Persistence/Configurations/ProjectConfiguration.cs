using AdminPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminPro.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Descripcion).HasMaxLength(500);

        builder.HasIndex(p => p.Nombre).IsUnique().HasFilter("[Activo] = 1").HasDatabaseName("IX_Projects_Nombre");

        // Rule INF-EF-003: Project -> Applications/BaseDeDatos cascade is a soft cascade
        // implemented in the Application layer command handler, NOT a hard SQL cascade.
        builder.HasMany(p => p.BasesDeDatos)
            .WithOne(d => d.Project)
            .HasForeignKey(d => d.ProyectoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Applications)
            .WithOne(a => a.Project)
            .HasForeignKey(a => a.ProyectoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(p => p.Activo);
    }
}
