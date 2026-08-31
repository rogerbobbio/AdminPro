using AdminPro.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Projects.Commands.CreateProject;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator(AppDbContext dbContext)
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100)
            .MustAsync(async (nombre, ct) =>
                !await dbContext.Projects.AnyAsync(p => p.Nombre == nombre, ct))
            .WithMessage("Ya existe un proyecto con ese nombre.");

        RuleFor(x => x.Descripcion)
            .MaximumLength(500);
    }
}
