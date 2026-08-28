using AdminPro.Domain.Interfaces;

namespace AdminPro.Domain.Entities;

public class Reporte : IAuditableEntity
{
    public int Id { get; set; }
    public int AplicacionId { get; set; }
    public string ReportCode { get; set; } = string.Empty;
    public string ReportName { get; set; } = string.Empty;
    public string? RegionId { get; set; }
    public string? ReportPath { get; set; }
    public string? SpTranship { get; set; }
    public string? SpReportViewer { get; set; }
    public string? Notas { get; set; }
    public string? ParametrosEjemplo { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Application Aplicacion { get; set; } = null!;
}
