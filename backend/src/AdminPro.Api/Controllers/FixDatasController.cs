using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.FixDatas.Commands.DeleteFixData;
using AdminPro.Application.FixDatas.Commands.UpdateFixData;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AdminPro.Api.Controllers;

[Route("api/fixdatas")]
public class FixDatasController(ISender sender) : ApiController(sender)
{
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateFixDataCommand command, CancellationToken ct)
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
        await Sender.Send(new DeleteFixDataCommand(id), ct);
        return NoContent();
    }
}
