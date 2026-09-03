using AdminPro.Application.Common;

namespace AdminPro.Application.Documentos.Commands.UpdateDocumento;

public record UpdateDocumentoCommand(
    int Id,
    string NombreArchivo,
    string UrlOneDrive,
    string Tipo,
    string? Descripcion,
    int Orden) : ICommand;
