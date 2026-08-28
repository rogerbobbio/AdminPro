using AdminPro.Domain.Interfaces;

namespace AdminPro.Domain.Entities;

public class Modulo : IAuditableEntity
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Icono { get; set; }
    public string RutaBase { get; set; } = string.Empty;
    public string? Color { get; set; }
    public int Orden { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
