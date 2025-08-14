using Rpssl.Application.Abstractions;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Stats.Queries;

public sealed record GetStatsQuery() : IRequest<Result<StatsDto>>;
