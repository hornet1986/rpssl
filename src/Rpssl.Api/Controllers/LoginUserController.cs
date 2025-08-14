using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rpssl.Api.Extensions;
using Rpssl.Application.Abstractions;
using Rpssl.Application.Users;
using Rpssl.Application.Users.Login;
using Rpssl.SharedKernel;
using Swashbuckle.AspNetCore.Annotations;

namespace Rpssl.Api.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class LoginUserController(ISender mediator) : ControllerBase
{
    public sealed record LoginRequest(string Username, string Password);

    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(Description = "Authenticates a user and issues tokens.")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        Result<UserDto> result = await mediator.Send(new LoginUserCommand(request.Username, request.Password), ct);
        return result.ToActionResult();
    }
}
