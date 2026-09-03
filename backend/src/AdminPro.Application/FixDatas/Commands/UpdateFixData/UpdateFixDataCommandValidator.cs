using FluentValidation;

namespace AdminPro.Application.FixDatas.Commands.UpdateFixData;

public class UpdateFixDataCommandValidator : AbstractValidator<UpdateFixDataCommand>
{
    public UpdateFixDataCommandValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Descripcion).MaximumLength(500);
        RuleFor(x => x.Orden).GreaterThanOrEqualTo(0);
    }
}
