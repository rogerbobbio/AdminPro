using System.Collections.Generic;
using AdminPro.Application.Common.Exceptions;
using FluentAssertions;
using FluentValidation.Results;
using Xunit;

namespace AdminPro.Application.Tests.Common.Exceptions;

public class ValidationExceptionTests
{
    [Fact]
    public void GroupsFailuresByPropertyName()
    {
        var failures = new List<ValidationFailure>
        {
            new("Nombre", "El nombre es obligatorio."),
            new("Nombre", "Ya existe un proyecto con ese nombre."),
            new("Url", "La URL no es válida.")
        };

        var exception = new ValidationException(failures);

        exception.Errors.Should().ContainKey("Nombre");
        exception.Errors["Nombre"].Should().HaveCount(2);
        exception.Errors.Should().ContainKey("Url");
        exception.Errors["Url"].Should().ContainSingle().Which.Should().Be("La URL no es válida.");
    }

    [Fact]
    public void DefaultConstructor_HasNoErrors()
    {
        var exception = new ValidationException();

        exception.Errors.Should().BeEmpty();
        exception.Message.Should().Be("One or more validation failures have occurred.");
    }
}
