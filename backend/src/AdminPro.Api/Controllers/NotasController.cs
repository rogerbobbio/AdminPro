using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Notas.Commands.DeleteNota;
using AdminPro.Application.Notas.Commands.UpdateNota;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AdminPro.Api.Controllers;

[Route("api/notas")]
public class NotasController(ISender sender) : ApiController(sender)
{
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateNotaCommand command, CancellationToken ct)
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
        await Sender.Send(new DeleteNotaCommand(id), ct);
        return NoContent();
    }
}
