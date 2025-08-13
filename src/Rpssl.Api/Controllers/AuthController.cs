using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rpssl.Api.Extensions;
using Rpssl.Application.Abstractions;
using Rpssl.Application.Features.Auth;
using Rpssl.Application.Features.Auth.Commands;
using Rpssl.SharedKernel;
using Swashbuckle.AspNetCore.Annotations;

namespace Rpssl.Api.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController(ISender mediator) : ControllerBase
{
    public sealed record RegisterRequest(string Username, string Password);
    public sealed record LoginRequest(string Username, string Password);
    public sealed record RefreshRequest(string RefreshToken, string SessionId);

    [HttpPost("register")]
    [AllowAnonymous]
    [SwaggerOperation(Description = "Registers a new user and issues tokens.")]
    [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
    {
        Result<AuthResultDto> result = await mediator.Send(new RegisterUserCommand(request.Username, request.Password), ct);
        return result.ToActionResult();
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(Description = "Authenticates a user and issues tokens.")]
    [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        Result<AuthResultDto> result = await mediator.Send(new LoginUserCommand(request.Username, request.Password), ct);
        return result.ToActionResult();
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [SwaggerOperation(Description = "Rotates refresh token and returns new access token.")]
    [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Refresh(RefreshRequest request, CancellationToken ct)
    {
        Result<AuthResultDto> result = await mediator.Send(new RefreshTokenCommand(request.RefreshToken, request.SessionId), ct);
        return result.ToActionResult();
    }
}
