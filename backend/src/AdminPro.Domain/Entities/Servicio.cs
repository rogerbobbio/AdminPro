using AdminPro.Domain.Interfaces;

namespace AdminPro.Domain.Entities;

public class Servicio : IAuditableEntity
{
    public int Id { get; set; }
    public int? ProyectoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string? Ambiente { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Notas { get; set; }
    public bool EsGlobal { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Project? Project { get; set; }
    public ICollection<AplicacionServicio> AplicacionServicios { get; set; } = [];
}
