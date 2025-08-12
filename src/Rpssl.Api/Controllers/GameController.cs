using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rpssl.Api.Extensions;
using Rpssl.Application.Features.Game;
using Rpssl.Application.Features.Game.Commands;
using Rpssl.SharedKernel;

namespace Rpssl.Api.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class GameController(IMediator mediator) : ControllerBase
{
    public sealed class PlayRequest
    {
        [Required]
        public int? PlayerChoiceId { get; init; }
    }

    [HttpPost("play")]
    public async Task<IActionResult> Play([FromBody] PlayRequest request, CancellationToken ct)
    {
        Result<GameResultDto> result = await mediator.Send(new PlayGameCommand(request.PlayerChoiceId!.Value), ct);
        return result.ToActionResult();
    }
}
