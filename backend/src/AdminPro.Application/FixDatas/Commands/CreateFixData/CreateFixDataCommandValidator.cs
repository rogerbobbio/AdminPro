using FluentValidation;

namespace AdminPro.Application.FixDatas.Commands.CreateFixData;

public class CreateFixDataCommandValidator : AbstractValidator<CreateFixDataCommand>
{
    public CreateFixDataCommandValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Descripcion).MaximumLength(500);
        RuleFor(x => x.Orden).GreaterThanOrEqualTo(0);
    }
}
