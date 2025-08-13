using System.Security.Claims;

namespace Rpssl.Application.Abstractions;

public interface IJwtTokenService
{
    (string token, DateTimeOffset expiresAt, string jti) CreateAccessToken(Guid userId, string username, string sessionId, IEnumerable<Claim>? extraClaims = null);
    string GenerateRefreshTokenRaw();
    string HashRefreshToken(string rawToken);
}
