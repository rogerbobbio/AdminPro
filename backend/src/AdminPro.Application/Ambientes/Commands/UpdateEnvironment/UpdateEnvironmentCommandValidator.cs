using FluentValidation;

namespace AdminPro.Application.Ambientes.Commands.UpdateEnvironment;

public class UpdateEnvironmentCommandValidator : AbstractValidator<UpdateEnvironmentCommand>
{
    public UpdateEnvironmentCommandValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Orden).GreaterThanOrEqualTo(0);
    }
}
