using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Rpssl.Application.Abstractions;

namespace Rpssl.Infrastructure.Authentication;

internal sealed class JwtTokenService(IOptions<JwtOptions> opts, IDateTimeProvider dateTime) : IJwtTokenService
{
    private readonly JwtOptions _options = opts.Value;

    public (string token, DateTimeOffset expiresAt, string jti) CreateAccessToken(Guid userId, string username, string sessionId, IEnumerable<Claim>? extraClaims = null)
    {
        DateTimeOffset now = dateTime.UtcNow;
        DateTimeOffset expires = now.AddMinutes(_options.AccessTokenMinutes);
        string jti = Guid.NewGuid().ToString();

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, username),
            new(JwtRegisteredClaimNames.Jti, jti),
            new("sid", sessionId)
        ];
        if (extraClaims is not null)
        {
            claims.AddRange(extraClaims);
        }

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_options.SigningKey));
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expires.UtcDateTime,
            signingCredentials: creds);

        string raw = new JwtSecurityTokenHandler().WriteToken(token);
        return (raw, expires, jti);
    }

    public string GenerateRefreshTokenRaw()
    {
        byte[] bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes);
    }

    public string HashRefreshToken(string rawToken)
    {
        using var sha = SHA256.Create();
        byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
