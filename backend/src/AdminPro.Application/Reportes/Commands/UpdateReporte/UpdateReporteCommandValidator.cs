using AdminPro.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Reportes.Commands.UpdateReporte;

public class UpdateReporteCommandValidator : AbstractValidator<UpdateReporteCommand>
{
    public UpdateReporteCommandValidator(AppDbContext dbContext)
    {
        RuleFor(x => x.ReportCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.ReportName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.RegionId).MaximumLength(10);
        RuleFor(x => x.ReportPath).MaximumLength(200);

        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                var reporte = await dbContext.Reportes
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(r => r.Id == command.Id, ct);
                if (reporte is null)
                {
                    return true;
                }

                return !await dbContext.Reportes.AnyAsync(
                    r => r.AplicacionId == reporte.AplicacionId && r.ReportCode == command.ReportCode && r.Id != command.Id, ct);
            })
            .WithName("ReportCode")
            .WithMessage("Ya existe un reporte con ese código en esta aplicación.");
    }
}
