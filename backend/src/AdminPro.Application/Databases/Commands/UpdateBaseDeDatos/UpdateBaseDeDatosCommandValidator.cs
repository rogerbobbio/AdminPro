using FluentValidation;

namespace AdminPro.Application.Databases.Commands.UpdateBaseDeDatos;

public class UpdateBaseDeDatosCommandValidator : AbstractValidator<UpdateBaseDeDatosCommand>
{
    public UpdateBaseDeDatosCommandValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Servidor).MaximumLength(200);
        RuleFor(x => x.LoginName).MaximumLength(100);
        RuleFor(x => x.Ambiente).MaximumLength(50);
    }
}
