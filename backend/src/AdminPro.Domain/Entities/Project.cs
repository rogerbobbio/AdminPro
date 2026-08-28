using AdminPro.Domain.Interfaces;

namespace AdminPro.Domain.Entities;

public class Project : IAuditableEntity
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<BaseDeDatos> BasesDeDatos { get; set; } = [];
    public ICollection<Application> Applications { get; set; } = [];
    public ICollection<Servicio> Servicios { get; set; } = [];
}
