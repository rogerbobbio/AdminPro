using FluentValidation;

namespace AdminPro.Application.Ambientes.Commands.CreateEnvironment;

public class CreateEnvironmentCommandValidator : AbstractValidator<CreateEnvironmentCommand>
{
    public CreateEnvironmentCommandValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Url)
            .Matches(@"^https?://.+")
            .When(x => !string.IsNullOrEmpty(x.Url))
            .WithMessage("La URL debe ser una dirección absoluta http:// o https://.");
        RuleFor(x => x.Orden).GreaterThanOrEqualTo(0);
    }
}
