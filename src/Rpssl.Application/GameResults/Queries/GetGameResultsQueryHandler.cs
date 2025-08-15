using Rpssl.Application.Abstractions;
using Rpssl.Domain.GameOutcome;
using Rpssl.Domain.GameResults;
using Rpssl.SharedKernel;

namespace Rpssl.Application.GameResults.Queries;

internal sealed class GetGameResultsQueryHandler(IGameResultRepository repo)
    : IRequestHandler<GetGameResultsQuery, Result<GameResultsDto>>
{
    public async Task<Result<GameResultsDto>> Handle(GetGameResultsQuery request, CancellationToken cancellationToken)
    {
        int total = await repo.CountAsync(cancellationToken);
        IDictionary<GameOutcome, int> byOutcome = await repo.CountByOutcomeAsync(cancellationToken);

        int playerWins = byOutcome.TryGetValue(GameOutcome.PlayerWin, out int pw) ? pw : 0;
        int computerWins = byOutcome.TryGetValue(GameOutcome.ComputerWin, out int cw) ? cw : 0;
        int draws = byOutcome.TryGetValue(GameOutcome.Draw, out int d) ? d : 0;

        GameResultsDto stats = new(total, playerWins, computerWins, draws);
        return stats;
    }
}
