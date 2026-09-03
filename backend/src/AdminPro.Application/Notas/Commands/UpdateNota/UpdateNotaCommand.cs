using AdminPro.Application.Common;

namespace AdminPro.Application.Notas.Commands.UpdateNota;

public record UpdateNotaCommand(
    int Id,
    string Titulo,
    string Descripcion,
    int Orden) : ICommand;
