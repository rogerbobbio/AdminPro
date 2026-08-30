namespace AdminPro.Application.Modulos.Queries.GetModulos;

public record ModuloDto(
    int Id,
    string Nombre,
    string? Icono,
    string RutaBase,
    string? Color,
    int Orden);
