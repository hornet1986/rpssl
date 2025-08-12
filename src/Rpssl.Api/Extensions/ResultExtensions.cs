using Microsoft.AspNetCore.Mvc;
using Rpssl.SharedKernel;

namespace Rpssl.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result)
    {
        return result.IsSuccess ? new OkResult() : MapError(result.Error);
    }

    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        return result.IsSuccess ? new OkObjectResult(result.Value) : MapError(result.Error);
    }

    private static IActionResult MapError(Error error)
    {
        return error.Type switch
        {
            ErrorType.NotFound => new NotFoundObjectResult(error),
            ErrorType.Validation => new BadRequestObjectResult(error),
            ErrorType.Conflict => new ConflictObjectResult(error),
            ErrorType.Problem => new ObjectResult(error) { StatusCode = StatusCodes.Status500InternalServerError },
            _ => new BadRequestObjectResult(error)
        };
    }
}
