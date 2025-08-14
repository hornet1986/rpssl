namespace Rpssl.Application.Abstractions;

/// <summary>
/// Exposes JWT-related configuration values needed by the application layer without
/// referencing infrastructure configuration types directly.
/// </summary>
public interface IJwtOptionsAccessor
{
    /// <summary>
    /// Number of days a refresh token remains valid from the moment of issuance.
    /// </summary>
    int RefreshTokenDays { get; }
}
