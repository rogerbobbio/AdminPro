using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Databases.Commands.CreateBaseDeDatos;
using AdminPro.Application.Projects.Commands.CreateProject;
using AdminPro.Application.Projects.Commands.DeleteProject;
using AdminPro.Application.Projects.Commands.UpdateProject;
using AdminPro.Application.Projects.Queries.GetProjectById;
using AdminPro.Application.Projects.Queries.GetProjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AdminPro.Api.Controllers;

public class ProjectsController(ISender sender) : ApiController(sender)
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectSummaryDto>>> GetAll(
        [FromQuery] bool includeInactive, CancellationToken ct)
    {
        var projects = await Sender.Send(new GetProjectsQuery(includeInactive), ct);
        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDetailDto>> GetById(
        int id, [FromQuery] bool includeInactiveChildren, CancellationToken ct)
    {
        var project = await Sender.Send(new GetProjectByIdQuery(id, includeInactiveChildren), ct);
        return Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateProjectCommand command, CancellationToken ct)
    {
        var id = await Sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateProjectCommand command, CancellationToken ct)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await Sender.Send(command, ct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await Sender.Send(new DeleteProjectCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{projectId}/basesdedatos")]
    public async Task<ActionResult<int>> CreateDatabase(int projectId, CreateDatabaseRequest request, CancellationToken ct)
    {
        var command = new CreateBaseDeDatosCommand(
            projectId, request.Nombre, request.Servidor, request.DatabaseId, request.LoginName, request.Ambiente, request.Notas);
        var id = await Sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = projectId }, id);
    }

    public record CreateDatabaseRequest(
        string Nombre, string? Servidor, int? DatabaseId, string? LoginName, string? Ambiente, string? Notas);
}
