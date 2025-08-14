using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rpssl.Api.Extensions;
using Rpssl.Application.Abstractions;
using Rpssl.Application.Stats;
using Rpssl.Application.Stats.Queries;
using Rpssl.SharedKernel;
using Swashbuckle.AspNetCore.Annotations;

namespace Rpssl.Api.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class StatsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Description = "Retrieves overall game statistics including total games played, win/loss counts, and other relevant metrics.")]
    [Produces(System.Net.Mime.MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(StatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        Result<StatsDto> result = await mediator.Send(new GetStatsQuery(), ct);
        return result.ToActionResult();
    }
}
