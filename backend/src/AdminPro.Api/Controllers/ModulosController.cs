using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Modulos.Queries.GetModulos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AdminPro.Api.Controllers;

public class ModulosController(ISender sender) : ApiController(sender)
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ModuloDto>>> GetAll(CancellationToken ct)
    {
        var modulos = await Sender.Send(new GetModulosQuery(), ct);
        return Ok(modulos);
    }
}
