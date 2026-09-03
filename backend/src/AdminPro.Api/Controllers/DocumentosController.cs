using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Documentos.Commands.DeleteDocumento;
using AdminPro.Application.Documentos.Commands.UpdateDocumento;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AdminPro.Api.Controllers;

[Route("api/documentos")]
public class DocumentosController(ISender sender) : ApiController(sender)
{
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateDocumentoCommand command, CancellationToken ct)
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
        await Sender.Send(new DeleteDocumentoCommand(id), ct);
        return NoContent();
    }
}
