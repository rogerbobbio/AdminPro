using AdminPro.Application.Common;

namespace AdminPro.Application.Databases.Commands.CreateBaseDeDatos;

public record CreateBaseDeDatosCommand(
    int ProyectoId,
    string Nombre,
    string? Servidor,
    int? DatabaseId,
    string? LoginName,
    string? Password,
    string? Ambiente,
    string? Notas) : ICommand<int>;
