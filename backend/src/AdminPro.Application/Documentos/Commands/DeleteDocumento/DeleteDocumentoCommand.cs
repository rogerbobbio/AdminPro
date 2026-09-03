using AdminPro.Application.Common;

namespace AdminPro.Application.Documentos.Commands.DeleteDocumento;

public record DeleteDocumentoCommand(int Id) : ICommand;
