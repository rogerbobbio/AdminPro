using AdminPro.Application.Common;

namespace AdminPro.Application.Projects.Commands.UpdateProject;

public record UpdateProjectCommand(int Id, string Nombre, string? Descripcion) : ICommand;
