using AdminPro.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Applications.Commands.CreateApplication;

public class CreateApplicationCommandValidator : AbstractValidator<CreateApplicationCommand>
{
    public CreateApplicationCommandValidator(AppDbContext dbContext)
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100);

        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
                !await dbContext.Applications.AnyAsync(
                    a => a.ProyectoId == command.ProyectoId && a.Nombre == command.Nombre, ct))
            .WithName("Nombre")
            .WithMessage("Ya existe una aplicación con ese nombre en este proyecto.");

        RuleFor(x => x.TecnologiaFront).MaximumLength(100);
        RuleFor(x => x.TecnologiaBack).MaximumLength(100);
        RuleFor(x => x.RamaDesarrollo).MaximumLength(100);
        RuleFor(x => x.RutaLocal).MaximumLength(500);
        RuleFor(x => x.RutaGit).MaximumLength(500);
        RuleFor(x => x.Orden).GreaterThanOrEqualTo(0);
    }
}
