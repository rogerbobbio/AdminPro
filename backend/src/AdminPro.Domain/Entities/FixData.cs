using AdminPro.Domain.Interfaces;

namespace AdminPro.Domain.Entities;

public class FixData : IAuditableEntity
{
    public int Id { get; set; }
    public int AplicacionId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Script { get; set; }
    public int Orden { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Application Aplicacion { get; set; } = null!;
}
