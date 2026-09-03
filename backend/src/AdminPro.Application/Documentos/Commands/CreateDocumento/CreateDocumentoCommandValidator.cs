using FluentValidation;

namespace AdminPro.Application.Documentos.Commands.CreateDocumento;

public class CreateDocumentoCommandValidator : AbstractValidator<CreateDocumentoCommand>
{
    public CreateDocumentoCommandValidator()
    {
        RuleFor(x => x.NombreArchivo).NotEmpty().MaximumLength(200);
        RuleFor(x => x.UrlOneDrive)
            .NotEmpty()
            .Matches(@"^https?://.+")
            .WithMessage("La URL debe ser una dirección absoluta http:// o https://.");
        RuleFor(x => x.Tipo).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Descripcion).MaximumLength(500);
        RuleFor(x => x.Orden).GreaterThanOrEqualTo(0);
    }
}
