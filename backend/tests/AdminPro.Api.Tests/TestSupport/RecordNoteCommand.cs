using AdminPro.Application.Common;
using AdminPro.Domain.Entities;
using AdminPro.Infrastructure.Persistence;
using FluentValidation;
using MediatR;

namespace AdminPro.Api.Tests.TestSupport;

public record RecordNoteCommand(string Nombre) : ICommand<int>;

public class RecordNoteCommandValidator : AbstractValidator<RecordNoteCommand>
{
    public RecordNoteCommandValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty();
    }
}

public class RecordNoteCommandHandler(AppDbContext db) : IRequestHandler<RecordNoteCommand, int>
{
    public async Task<int> Handle(RecordNoteCommand request, CancellationToken cancellationToken)
    {
        var project = new Project
        {
            Nombre = request.Nombre,
            Activo = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync(cancellationToken);

        return project.Id;
    }
}
