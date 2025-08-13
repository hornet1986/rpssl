using System.Security.Claims;

namespace Rpssl.Application.Abstractions;

public interface IJwtTokenService
{
    /// <summary>
    /// Creates a signed JWT access token for the specified user / session.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="username">The username (typically used as the <c>sub</c> / name claim).</param>
    /// <param name="sessionId">A logical session identifier used to correlate refresh tokens and revoke sessions.</param>
    /// <param name="extraClaims">Optional additional claims to merge into the token.</param>
    /// <returns>A tuple containing the serialized token, its absolute expiry instant, and the token's JTI (unique identifier).</returns>
    (string token, DateTimeOffset expiresAt, string jti) CreateAccessToken(Guid userId, string username, string sessionId, IEnumerable<Claim>? extraClaims = null);

    /// <summary>
    /// Generates a new cryptographically strong raw refresh token value (not yet hashed).
    /// </summary>
    /// <returns>The raw refresh token string suitable for returning to a client (should be shown only once).</returns>
    string GenerateRefreshTokenRaw();

    /// <summary>
    /// Hashes a raw refresh token for persistent storage / comparison.
    /// </summary>
    /// <param name="rawToken">The raw refresh token previously issued to the client.</param>
    /// <returns>A deterministic hash representation safe to store server-side.</returns>
    string HashRefreshToken(string rawToken);
}
