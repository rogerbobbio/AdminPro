using AdminPro.Application.Common;

namespace AdminPro.Application.FixDatas.Commands.UpdateFixData;

public record UpdateFixDataCommand(
    int Id,
    string Nombre,
    string? Descripcion,
    string? Script,
    int Orden) : ICommand;
