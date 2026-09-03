using FluentValidation;

namespace AdminPro.Application.Notas.Commands.CreateNota;

public class CreateNotaCommandValidator : AbstractValidator<CreateNotaCommand>
{
    public CreateNotaCommandValidator()
    {
        RuleFor(x => x.Titulo).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Descripcion).NotEmpty();
        RuleFor(x => x.Orden).GreaterThanOrEqualTo(0);
    }
}
