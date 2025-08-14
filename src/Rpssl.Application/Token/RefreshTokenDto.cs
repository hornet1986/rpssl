namespace Rpssl.Application.Token;

public sealed record RefreshTokenDto(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt,
    string SessionId,
    string Username);
