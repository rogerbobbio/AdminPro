using AdminPro.Application.Common;

namespace AdminPro.Application.Applications.Commands.DeleteApplication;

public record DeleteApplicationCommand(int Id) : ICommand;
