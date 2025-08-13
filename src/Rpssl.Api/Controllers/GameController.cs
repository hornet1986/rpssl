using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Rpssl.Api.Extensions;
using Rpssl.Application.Abstractions;
using Rpssl.Application.Features.Game;
using Rpssl.Application.Features.Game.Commands;
using Rpssl.SharedKernel;
using Swashbuckle.AspNetCore.Annotations;

namespace Rpssl.Api.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class GameController(ISender mediator) : ControllerBase
{
    public sealed class PlayRequest
    {
        [Required]
        public int? PlayerChoiceId { get; init; }
    }

    [HttpPost("play")]
    [SwaggerOperation(Description = "Plays a round of Rock-Paper-Scissors-Spock-Lizard. The player submits their choice, and the server responds with the result of the game.")]
    [Produces(System.Net.Mime.MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GameResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Play([FromBody] PlayRequest request, CancellationToken ct)
    {
        Result<GameResultDto> result = await mediator.Send(new PlayGameCommand(request.PlayerChoiceId!.Value), ct);
        return result.ToActionResult();
    }
}
