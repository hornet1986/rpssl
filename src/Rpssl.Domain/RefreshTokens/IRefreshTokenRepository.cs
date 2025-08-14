namespace Rpssl.Domain.RefreshTokens;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct = default);
    Task AddAsync(RefreshToken token, CancellationToken ct = default);
    Task UpdateAsync(RefreshToken token, CancellationToken ct = default);
    Task<IReadOnlyList<RefreshToken>> GetActiveByUserAndSessionAsync(Guid userId, string sessionId, CancellationToken ct = default);
}
