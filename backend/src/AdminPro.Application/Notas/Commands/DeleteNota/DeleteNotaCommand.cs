using AdminPro.Application.Common;

namespace AdminPro.Application.Notas.Commands.DeleteNota;

public record DeleteNotaCommand(int Id) : ICommand;
