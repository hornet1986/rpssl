using Rpssl.Application.Abstractions;
using Rpssl.SharedKernel;

namespace Rpssl.Application.GameResults.Queries;

public sealed record GetGameResultsQuery() : IRequest<Result<GameResultsDto>>;
