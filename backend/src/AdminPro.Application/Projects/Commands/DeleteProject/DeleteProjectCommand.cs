using AdminPro.Application.Common;

namespace AdminPro.Application.Projects.Commands.DeleteProject;

public record DeleteProjectCommand(int Id) : ICommand;
