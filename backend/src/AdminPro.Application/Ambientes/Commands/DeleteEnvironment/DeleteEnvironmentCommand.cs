using AdminPro.Application.Common;

namespace AdminPro.Application.Ambientes.Commands.DeleteEnvironment;

public record DeleteEnvironmentCommand(int Id) : ICommand;
