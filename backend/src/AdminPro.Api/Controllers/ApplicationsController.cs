using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Ambientes.Commands.CreateEnvironment;
using AdminPro.Application.Applications.Commands.DeleteApplication;
using AdminPro.Application.Applications.Commands.UpdateApplication;
using AdminPro.Application.Applications.Queries.GetApplicationById;
using AdminPro.Application.Reportes.Commands.CreateReporte;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AdminPro.Api.Controllers;

public class ApplicationsController(ISender sender) : ApiController(sender)
{
    [HttpGet("{id}")]
    public async Task<ActionResult<ApplicationDetailDto>> GetById(int id, CancellationToken ct)
    {
        var application = await Sender.Send(new GetApplicationByIdQuery(id), ct);
        return Ok(application);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateApplicationCommand command, CancellationToken ct)
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
        await Sender.Send(new DeleteApplicationCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{appId}/ambientes")]
    public async Task<ActionResult<int>> CreateEnvironment(int appId, CreateEnvironmentRequest request, CancellationToken ct)
    {
        var command = new CreateEnvironmentCommand(appId, request.Nombre, request.Url, request.EsWebApi, request.Notas, request.Orden);
        var id = await Sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = appId }, id);
    }

    public record CreateEnvironmentRequest(string Nombre, string? Url, bool EsWebApi, string? Notas, int Orden);

    [HttpPost("{appId}/reportes")]
    public async Task<ActionResult<int>> CreateReporte(int appId, CreateReporteRequest request, CancellationToken ct)
    {
        var command = new CreateReporteCommand(
            appId,
            request.ReportCode,
            request.ReportName,
            request.RegionId,
            request.ReportPath,
            request.SpTranship,
            request.SpReportViewer,
            request.Notas,
            request.ParametrosEjemplo);
        var id = await Sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = appId }, id);
    }

    public record CreateReporteRequest(
        string ReportCode,
        string ReportName,
        string? RegionId,
        string? ReportPath,
        string? SpTranship,
        string? SpReportViewer,
        string? Notas,
        string? ParametrosEjemplo);
}
