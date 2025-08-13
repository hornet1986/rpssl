namespace Rpssl.Domain.Entities;

public class User
{
    private User() { }

    public User(string username, string passwordHash, string securityStamp)
    {
        Username = username;
        PasswordHash = passwordHash;
        SecurityStamp = securityStamp;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Username { get; private set; } = null!; // unique
    public string PasswordHash { get; private set; } = null!;
    public string SecurityStamp { get; private set; } = null!; // rotate on credential change
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastLoginAt { get; private set; }
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
    private readonly List<RefreshToken> _refreshTokens = [];

    public void SetLastLogin(DateTimeOffset at) => LastLoginAt = at;
    public void AddRefreshToken(RefreshToken token) => _refreshTokens.Add(token);
}
