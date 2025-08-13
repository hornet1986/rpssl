using Rpssl.Application.Abstractions;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Features.Stats.Queries;

public sealed record GetStatsQuery() : IRequest<Result<StatsDto>>;
