using AdminPro.Domain.Interfaces;

namespace AdminPro.Domain.Entities;

public class BaseDeDatos : IAuditableEntity
{
    public int Id { get; set; }
    public int ProyectoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Servidor { get; set; }
    public int? DatabaseId { get; set; }
    public string? LoginName { get; set; }
    public string? Ambiente { get; set; }
    public string? Notas { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Project Project { get; set; } = null!;
}
