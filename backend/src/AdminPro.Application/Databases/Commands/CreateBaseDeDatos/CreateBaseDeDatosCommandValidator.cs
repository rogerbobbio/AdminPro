using FluentValidation;

namespace AdminPro.Application.Databases.Commands.CreateBaseDeDatos;

public class CreateBaseDeDatosCommandValidator : AbstractValidator<CreateBaseDeDatosCommand>
{
    public CreateBaseDeDatosCommandValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Servidor).MaximumLength(200);
        RuleFor(x => x.LoginName).MaximumLength(100);
        RuleFor(x => x.Password).MaximumLength(200);
        RuleFor(x => x.Ambiente).MaximumLength(50);
    }
}
