using FluentValidation;

namespace AdminPro.Application.Ambientes.Commands.CreateEnvironment;

public class CreateEnvironmentCommandValidator : AbstractValidator<CreateEnvironmentCommand>
{
    public CreateEnvironmentCommandValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Orden).GreaterThanOrEqualTo(0);
    }
}
