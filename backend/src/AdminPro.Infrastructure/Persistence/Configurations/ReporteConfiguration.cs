using AdminPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminPro.Infrastructure.Persistence.Configurations;

public class ReporteConfiguration : IEntityTypeConfiguration<Reporte>
{
    public void Configure(EntityTypeBuilder<Reporte> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.ReportCode).HasMaxLength(20).IsRequired();
        builder.Property(r => r.ReportName).HasMaxLength(200).IsRequired();
        builder.Property(r => r.RegionId).HasMaxLength(10);
        builder.Property(r => r.ReportPath).HasMaxLength(200);
        builder.Property(r => r.SpTranship).HasMaxLength(200);
        builder.Property(r => r.SpReportViewer).HasMaxLength(200);

        builder.HasOne(r => r.Aplicacion)
            .WithMany(a => a.Reportes)
            .HasForeignKey(r => r.AplicacionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => new { r.AplicacionId, r.ReportCode })
            .IsUnique()
            .HasFilter("[Activo] = 1")
            .HasDatabaseName("IX_Reportes_AplicacionId_ReportCode");

        builder.HasQueryFilter(r => r.Activo);
    }
}
