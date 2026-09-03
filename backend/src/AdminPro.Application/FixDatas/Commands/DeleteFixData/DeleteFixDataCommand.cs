using AdminPro.Application.Common;

namespace AdminPro.Application.FixDatas.Commands.DeleteFixData;

public record DeleteFixDataCommand(int Id) : ICommand;
