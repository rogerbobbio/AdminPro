using AdminPro.Application.Common;

namespace AdminPro.Application.Ambientes.Commands.UpdateEnvironment;

public record UpdateEnvironmentCommand(
    int Id,
    string Nombre,
    string? Url,
    bool EsWebApi,
    string? Notas,
    int Orden) : ICommand;
