using AdminPro.Application.Common;

namespace AdminPro.Application.Projects.Commands.CreateProject;

public record CreateProjectCommand(string Nombre, string? Descripcion) : ICommand<int>;
