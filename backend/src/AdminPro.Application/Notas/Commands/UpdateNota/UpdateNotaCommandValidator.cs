using FluentValidation;

namespace AdminPro.Application.Notas.Commands.UpdateNota;

public class UpdateNotaCommandValidator : AbstractValidator<UpdateNotaCommand>
{
    public UpdateNotaCommandValidator()
    {
        RuleFor(x => x.Titulo).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Descripcion).NotEmpty();
        RuleFor(x => x.Orden).GreaterThanOrEqualTo(0);
    }
}
