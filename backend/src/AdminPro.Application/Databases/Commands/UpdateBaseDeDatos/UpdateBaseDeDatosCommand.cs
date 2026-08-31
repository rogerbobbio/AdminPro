using AdminPro.Application.Common;

namespace AdminPro.Application.Databases.Commands.UpdateBaseDeDatos;

public record UpdateBaseDeDatosCommand(
    int Id,
    string Nombre,
    string? Servidor,
    int? DatabaseId,
    string? LoginName,
    string? Ambiente,
    string? Notas) : ICommand;
