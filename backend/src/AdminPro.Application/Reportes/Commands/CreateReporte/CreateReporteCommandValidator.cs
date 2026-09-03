using AdminPro.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Reportes.Commands.CreateReporte;

public class CreateReporteCommandValidator : AbstractValidator<CreateReporteCommand>
{
    public CreateReporteCommandValidator(AppDbContext dbContext)
    {
        RuleFor(x => x.ReportCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.ReportName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.RegionId).MaximumLength(10);
        RuleFor(x => x.ReportPath).MaximumLength(200);

        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
                !await dbContext.Reportes.AnyAsync(
                    r => r.AplicacionId == command.AplicacionId && r.ReportCode == command.ReportCode, ct))
            .WithName("ReportCode")
            .WithMessage("Ya existe un reporte con ese código en esta aplicación.");
    }
}
