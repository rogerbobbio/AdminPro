using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Databases.Commands.DeleteBaseDeDatos;
using AdminPro.Application.Databases.Commands.UpdateBaseDeDatos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AdminPro.Api.Controllers;

[Route("api/basesdedatos")]
public class BaseDeDatosController(ISender sender) : ApiController(sender)
{
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateBaseDeDatosCommand command, CancellationToken ct)
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
        await Sender.Send(new DeleteBaseDeDatosCommand(id), ct);
        return NoContent();
    }
}
