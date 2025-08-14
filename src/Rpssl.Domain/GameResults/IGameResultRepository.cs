namespace Rpssl.Domain.GameResults;

public interface IGameResultRepository
{
    Task AddAsync(GameResult result, CancellationToken ct = default);
    Task<GameResult?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<GameResult>> GetRecentAsync(int take, CancellationToken ct = default);
    Task<int> CountAsync(CancellationToken ct = default);
    Task<IDictionary<GameOutcome.GameOutcome, int>> CountByOutcomeAsync(CancellationToken ct = default);
}
