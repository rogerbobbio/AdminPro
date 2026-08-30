using System.Threading;
using System.Threading.Tasks;
using AdminPro.Application.Dashboard.Queries.GetDashboardSummary;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AdminPro.Api.Controllers;

public class DashboardController(ISender sender) : ApiController(sender)
{
    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary(CancellationToken ct)
    {
        var summary = await Sender.Send(new GetDashboardSummaryQuery(), ct);
        return Ok(summary);
    }
}
