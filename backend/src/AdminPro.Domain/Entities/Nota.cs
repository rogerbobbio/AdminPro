using AdminPro.Domain.Interfaces;

namespace AdminPro.Domain.Entities;

public class Nota : IAuditableEntity
{
    public int Id { get; set; }
    public int AplicacionId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int Orden { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Application Aplicacion { get; set; } = null!;
}
