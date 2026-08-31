using AdminPro.Application.Common.Exceptions;
using AdminPro.Domain.Exceptions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace AdminPro.Api.Tests.TestSupport;

[ApiController]
[Route("api/testonly")]
public class TestOnlyController : ControllerBase
{
    [HttpGet("validation-error")]
    public IActionResult ValidationError()
    {
        throw new ValidationException(
        [
            new ValidationFailure("Nombre", "El nombre es obligatorio.")
        ]);
    }

    [HttpGet("unhandled-error")]
    public IActionResult UnhandledError()
    {
        throw new InvalidOperationException("boom");
    }

    [HttpGet("not-found-error")]
    public IActionResult NotFoundError()
    {
        throw new NotFoundException("Project 1 not found.");
    }

    [HttpGet("domain-error")]
    public IActionResult DomainError()
    {
        throw new DomainException("Project name already exists.");
    }
}
