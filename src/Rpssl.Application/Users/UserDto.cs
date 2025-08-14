namespace Rpssl.Application.Users;

public sealed record UserDto(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt,
    string SessionId,
    string Username);
