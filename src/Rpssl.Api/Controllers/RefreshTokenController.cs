using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rpssl.Api.Extensions;
using Rpssl.Application.Abstractions;
using Rpssl.Application.Token;
using Rpssl.SharedKernel;
using Swashbuckle.AspNetCore.Annotations;

namespace Rpssl.Api.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class RefreshTokenController(ISender mediator) : ControllerBase
{
    public sealed record RefreshRequest(string RefreshToken, string SessionId);

    [HttpPost("refresh")]
    [AllowAnonymous]
    [SwaggerOperation(Description = "Rotates refresh token and returns new access token.")]
    [ProducesResponseType(typeof(RefreshTokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Refresh(RefreshRequest request, CancellationToken ct)
    {
        Result<RefreshTokenDto> result = await mediator.Send(new RefreshTokenCommand(request.RefreshToken, request.SessionId), ct);
        return result.ToActionResult();
    }
}
