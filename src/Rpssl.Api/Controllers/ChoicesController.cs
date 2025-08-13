using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Rpssl.Api.Extensions;
using Rpssl.Application.Abstractions;
using Rpssl.Application.Features.Choices;
using Rpssl.Application.Features.Choices.Queries;
using Rpssl.SharedKernel;
using Swashbuckle.AspNetCore.Annotations;

namespace Rpssl.Api.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class ChoicesController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Description = "Retrieves a list of all available choices for the game.")]
    [Produces(System.Net.Mime.MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IReadOnlyList<ChoiceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        Result<IReadOnlyList<ChoiceDto>> result = await mediator.Send(new GetChoicesQuery(), ct);
        return result.ToActionResult();
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Description = "Retrieves a specific choice by its ID.")]
    [Produces(System.Net.Mime.MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ChoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        Result<ChoiceDto> result = await mediator.Send(new GetChoiceByIdQuery(id), ct);
        return result.ToActionResult();
    }
}
