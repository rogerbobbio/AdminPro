namespace AdminPro.Domain.Entities;

public class AplicacionServicio
{
    public int AplicacionId { get; set; }
    public int ServicioId { get; set; }
    public string? NotasEspecificas { get; set; }
    public DateTime CreatedAt { get; set; }

    public Application Aplicacion { get; set; } = null!;
    public Servicio Servicio { get; set; } = null!;
}
