using AdminPro.Application.Common;

namespace AdminPro.Application.Applications.Commands.UpdateApplication;

public record UpdateApplicationCommand(
    int Id,
    string Nombre,
    string? Descripcion,
    string? TecnologiaFront,
    string? TecnologiaBack,
    string? RamaDesarrollo,
    string? ApplicationName,
    string? RutaLocal,
    string? RutaGit,
    string? ComoSeLevanta,
    string? NotasCompilacion,
    int Orden) : ICommand;
