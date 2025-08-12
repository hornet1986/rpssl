using Microsoft.EntityFrameworkCore;
using Rpssl.Domain.Entities;
using Rpssl.Domain.Enums;
using Rpssl.Domain.Repositories;

namespace Rpssl.Infrastructure.Database.Repositories;

public class EfGameResultRepository(RpsslDbContext db) : IGameResultRepository
{
    public async Task AddAsync(GameResult result, CancellationToken ct = default)
    {
        await db.GameResults.AddAsync(result, ct);
    }

    public Task<GameResult?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.GameResults
            .Include(r => r.PlayerChoice)
            .Include(r => r.ComputerChoice)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<IReadOnlyList<GameResult>> GetRecentAsync(int take, CancellationToken ct = default)
    {
        return await db.GameResults
            .OrderByDescending(r => r.PlayedAt)
            .Take(take)
            .Include(r => r.PlayerChoice)
            .Include(r => r.ComputerChoice)
            .ToListAsync(ct);
    }

    public Task<int> CountAsync(CancellationToken ct = default)
        => db.GameResults.CountAsync(ct);

    public async Task<IDictionary<GameOutcome, int>> CountByOutcomeAsync(CancellationToken ct = default)
    {
        return await db.GameResults
            .GroupBy(r => r.Outcome)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count, ct);
    }
}
