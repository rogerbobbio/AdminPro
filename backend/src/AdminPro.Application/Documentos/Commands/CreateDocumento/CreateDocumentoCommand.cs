using AdminPro.Application.Common;

namespace AdminPro.Application.Documentos.Commands.CreateDocumento;

public record CreateDocumentoCommand(
    int AplicacionId,
    string NombreArchivo,
    string UrlOneDrive,
    string Tipo,
    string? Descripcion,
    int Orden) : ICommand<int>;
