using Microsoft.EntityFrameworkCore;
using Rpssl.Domain.RefreshTokens;

namespace Rpssl.Infrastructure.Database.Repositories;

internal sealed class EfRefreshTokenRepository(RpsslDbContext db) : IRefreshTokenRepository
{
    public Task<RefreshToken?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.RefreshTokens.FirstOrDefaultAsync(t => t.Id == id, ct);

    public Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct = default)
        => db.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

    public async Task AddAsync(RefreshToken token, CancellationToken ct = default)
        => await db.RefreshTokens.AddAsync(token, ct);

    public Task UpdateAsync(RefreshToken token, CancellationToken ct = default)
    {
        db.RefreshTokens.Update(token);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserAndSessionAsync(Guid userId, string sessionId, CancellationToken ct = default)
        => await db.RefreshTokens
            .Where(t => t.UserId == userId && t.SessionId == sessionId && t.RevokedAt == null && t.ExpiresAt > DateTimeOffset.UtcNow)
            .ToListAsync(ct);
}
