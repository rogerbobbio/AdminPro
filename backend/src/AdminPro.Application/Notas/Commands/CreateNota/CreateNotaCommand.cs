using AdminPro.Application.Common;

namespace AdminPro.Application.Notas.Commands.CreateNota;

public record CreateNotaCommand(
    int AplicacionId,
    string Titulo,
    string Descripcion,
    int Orden) : ICommand<int>;
