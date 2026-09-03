using AdminPro.Domain.Interfaces;

namespace AdminPro.Domain.Entities;

public class Application : IAuditableEntity
{
    public int Id { get; set; }
    public int ProyectoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Tipo { get; set; }
    public string? TecnologiaFront { get; set; }
    public string? TecnologiaBack { get; set; }
    public string? RamaDesarrollo { get; set; }
    public string? ApplicationName { get; set; }
    public string? RutaLocal { get; set; }
    public string? RutaGit { get; set; }
    public string? ComoSeLevanta { get; set; }
    public string? NotasCompilacion { get; set; }
    public int Orden { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Project Project { get; set; } = null!;
    public ICollection<Ambiente> Ambientes { get; set; } = [];
    public ICollection<Reporte> Reportes { get; set; } = [];
    public ICollection<Nota> Notas { get; set; } = [];
    public ICollection<Documento> Documentos { get; set; } = [];
    public ICollection<FixData> FixDatas { get; set; } = [];
    public ICollection<AplicacionServicio> AplicacionServicios { get; set; } = [];
}
