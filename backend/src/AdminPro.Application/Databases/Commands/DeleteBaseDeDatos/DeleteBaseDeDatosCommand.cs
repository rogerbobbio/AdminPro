using AdminPro.Application.Common;

namespace AdminPro.Application.Databases.Commands.DeleteBaseDeDatos;

public record DeleteBaseDeDatosCommand(int Id) : ICommand;
