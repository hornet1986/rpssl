using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rpssl.Api.Extensions;
using Rpssl.Application.Features.Choices;
using Rpssl.Application.Features.Choices.Queries;
using Rpssl.SharedKernel;

namespace Rpssl.Api.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class ChoicesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        Result<IReadOnlyList<ChoiceDto>> result = await mediator.Send(new GetChoicesQuery(), ct);
        return result.ToActionResult();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        Result<ChoiceDto> result = await mediator.Send(new GetChoiceByIdQuery(id), ct);
        return result.ToActionResult();
    }
}
