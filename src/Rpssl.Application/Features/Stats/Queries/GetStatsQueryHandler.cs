using MediatR;
using Rpssl.Domain.Enums;
using Rpssl.Domain.Repositories;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Features.Stats.Queries;

internal sealed class GetStatsQueryHandler(IGameResultRepository repo)
    : IRequestHandler<GetStatsQuery, Result<StatsDto>>
{
    public async Task<Result<StatsDto>> Handle(GetStatsQuery request, CancellationToken cancellationToken)
    {
    int total = await repo.CountAsync(cancellationToken);
    IDictionary<GameOutcome, int> byOutcome = await repo.CountByOutcomeAsync(cancellationToken);

    int playerWins = byOutcome.TryGetValue(GameOutcome.PlayerWin, out int pw) ? pw : 0;
    int computerWins = byOutcome.TryGetValue(GameOutcome.ComputerWin, out int cw) ? cw : 0;
    int draws = byOutcome.TryGetValue(GameOutcome.Draw, out int d) ? d : 0;

    StatsDto stats = new(total, playerWins, computerWins, draws);
    return Result.Success(stats);
    }
}
