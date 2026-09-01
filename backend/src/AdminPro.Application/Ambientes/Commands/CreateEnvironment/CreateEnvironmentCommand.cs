using AdminPro.Application.Common;

namespace AdminPro.Application.Ambientes.Commands.CreateEnvironment;

public record CreateEnvironmentCommand(
    int AplicacionId,
    string Nombre,
    string? Url,
    bool EsWebApi,
    string? Notas,
    int Orden) : ICommand<int>;
