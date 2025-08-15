using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rpssl.Api.Extensions;
using Rpssl.Application.Abstractions;
using Rpssl.Application.GameResults;
using Rpssl.Application.GameResults.Queries;
using Rpssl.Application.Games;
using Rpssl.SharedKernel;
using Swashbuckle.AspNetCore.Annotations;

namespace Rpssl.Api.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class GameResultsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Description = "Retrieves overall game results including total games played, win/loss counts, and other relevant metrics.")]
    [Produces(System.Net.Mime.MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GameResultsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        Result<GameResultsDto> result = await mediator.Send(new GetGameResultsQuery(), ct);
        return result.ToActionResult();
    }

    [HttpDelete]
    [SwaggerOperation(Description = "Deletes all stored game results.")]
    [Produces(System.Net.Mime.MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(CancellationToken ct)
    {
        Result result = await mediator.Send(new ClearGameResultsCommand(), ct);
        return result.ToActionResult();
    }
}
