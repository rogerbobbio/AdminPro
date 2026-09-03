using AdminPro.Application.Common;

namespace AdminPro.Application.FixDatas.Commands.CreateFixData;

public record CreateFixDataCommand(
    int AplicacionId,
    string Nombre,
    string? Descripcion,
    string? Script,
    int Orden) : ICommand<int>;
