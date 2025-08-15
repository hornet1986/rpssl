namespace Rpssl.Domain.GameResults;

public interface IGameResultRepository
{
    /// <summary>
    /// Persists a new <see cref="GameResult"/> instance.
    /// </summary>
    /// <param name="result">The game result to add.</param>
    /// <param name="ct">Cancellation token.</param>
    Task AddAsync(GameResult result, CancellationToken ct = default);
    /// <summary>
    /// Retrieves a single <see cref="GameResult"/> by its identifier including related choices.
    /// </summary>
    /// <param name="id">Game result identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The matching game result or <c>null</c> if not found.</returns>
    Task<GameResult?> GetByIdAsync(Guid id, CancellationToken ct = default);
    /// <summary>
    /// Returns the most recent game results ordered descending by played time.
    /// </summary>
    /// <param name="take">Number of results to return.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Readonly list of recent game results.</returns>
    Task<IReadOnlyList<GameResult>> GetRecentAsync(int take, CancellationToken ct = default);
    /// <summary>
    /// Counts all stored game results.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Total count.</returns>
    Task<int> CountAsync(CancellationToken ct = default);
    /// <summary>
    /// Counts game results grouped by <see cref="GameOutcome.GameOutcome"/> value.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A dictionary mapping outcome to occurrence count.</returns>
    Task<IDictionary<GameOutcome.GameOutcome, int>> CountByOutcomeAsync(CancellationToken ct = default);
    /// <summary>
    /// Deletes all game results.
    /// </summary>
    /// <returns>The number of deleted rows.</returns>
    Task<int> DeleteAllAsync(CancellationToken ct = default);
}
