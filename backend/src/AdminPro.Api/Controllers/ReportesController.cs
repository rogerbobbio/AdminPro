using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Reportes.Commands.DeleteReporte;
using AdminPro.Application.Reportes.Commands.UpdateReporte;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AdminPro.Api.Controllers;

[Route("api/reportes")]
public class ReportesController(ISender sender) : ApiController(sender)
{
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateReporteCommand command, CancellationToken ct)
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
        await Sender.Send(new DeleteReporteCommand(id), ct);
        return NoContent();
    }
}
