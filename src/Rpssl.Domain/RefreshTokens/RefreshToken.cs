namespace Rpssl.Domain.RefreshTokens;

public class RefreshToken
{
    private RefreshToken() { }

    public RefreshToken(Guid userId, string tokenHash, string sessionId, DateTimeOffset createdAt, DateTimeOffset expiresAt)
    {
        UserId = userId;
        TokenHash = tokenHash;
        SessionId = sessionId;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = null!; // SHA256 hash of the raw refresh token
    public string SessionId { get; private set; } = null!; // groups tokens issued for a device/session
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public Guid? ReplacedByTokenId { get; private set; }

    public bool IsActive(DateTimeOffset now) => RevokedAt is null && now < ExpiresAt;
    public void Revoke(DateTimeOffset at) => RevokedAt = at;
    public void ReplaceBy(Guid newTokenId) => ReplacedByTokenId = newTokenId;
}
