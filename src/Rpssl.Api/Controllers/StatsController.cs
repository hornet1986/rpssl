using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rpssl.Api.Extensions;
using Rpssl.Application.Features.Stats;
using Rpssl.Application.Features.Stats.Queries;
using Rpssl.SharedKernel;

namespace Rpssl.Api.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class StatsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        Result<StatsDto> result = await mediator.Send(new GetStatsQuery(), ct);
        return result.ToActionResult();
    }
}
