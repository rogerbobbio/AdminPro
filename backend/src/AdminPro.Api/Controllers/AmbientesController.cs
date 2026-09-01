using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Ambientes.Commands.DeleteEnvironment;
using AdminPro.Application.Ambientes.Commands.UpdateEnvironment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AdminPro.Api.Controllers;

[Route("api/ambientes")]
public class AmbientesController(ISender sender) : ApiController(sender)
{
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateEnvironmentCommand command, CancellationToken ct)
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
        await Sender.Send(new DeleteEnvironmentCommand(id), ct);
        return NoContent();
    }
}
