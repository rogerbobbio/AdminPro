using AdminPro.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AdminPro.Application.Projects.Commands.UpdateProject;

public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator(AppDbContext dbContext)
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100);

        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
                !await dbContext.Projects.AnyAsync(p => p.Nombre == command.Nombre && p.Id != command.Id, ct))
            .WithName("Nombre")
            .WithMessage("Ya existe un proyecto con ese nombre.");

        RuleFor(x => x.Descripcion)
            .MaximumLength(500);
    }
}
