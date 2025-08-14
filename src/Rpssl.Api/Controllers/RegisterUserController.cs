using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rpssl.Api.Extensions;
using Rpssl.Application.Abstractions;
using Rpssl.Application.Users;
using Rpssl.Application.Users.Register;
using Rpssl.SharedKernel;
using Swashbuckle.AspNetCore.Annotations;

namespace Rpssl.Api.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class RegisterUserController(ISender mediator) : ControllerBase
{
    public sealed record RegisterRequest(string Username, string Password);

    [HttpPost("register")]
    [AllowAnonymous]
    [SwaggerOperation(Description = "Registers a new user and issues tokens.")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
    {
        Result<UserDto> result = await mediator.Send(new RegisterUserCommand(request.Username, request.Password), ct);
        return result.ToActionResult();
    }
}
