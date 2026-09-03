using AdminPro.Application.Common;

namespace AdminPro.Application.Reportes.Commands.DeleteReporte;

public record DeleteReporteCommand(int Id) : ICommand;
