namespace AdminPro.Domain.Interfaces;

public interface IAuditableEntity
{
    int Id { get; set; }
    bool Activo { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}
