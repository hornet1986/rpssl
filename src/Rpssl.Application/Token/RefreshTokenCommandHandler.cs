using Rpssl.Application.Abstractions;
using Rpssl.Domain.RefreshTokens;
using Rpssl.Domain.UnitOfWork;
using Rpssl.Domain.Users;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Token;

internal sealed class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshRepo,
    IUserRepository users,
    IJwtTokenService jwt,
    IDateTimeProvider dateTime,
    IUnitOfWork uow) : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenDto>>
{
    private static readonly Error InvalidRefresh = Error.Validation("Auth.InvalidRefresh", "Invalid or expired refresh token");

    public async Task<Result<RefreshTokenDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        string hash = jwt.HashRefreshToken(request.RefreshToken);
        RefreshToken? existing = await refreshRepo.GetByHashAsync(hash, cancellationToken);
        if (existing is null || !existing.IsActive(dateTime.UtcNow) || existing.SessionId != request.SessionId)
        {
            return Result.Failure<RefreshTokenDto>(InvalidRefresh);
        }

        User? user = await users.GetByIdAsync(existing.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<RefreshTokenDto>(InvalidRefresh);
        }

        // Rotate
        existing.Revoke(dateTime.UtcNow);
        string sessionId = existing.SessionId; // same session
        (string accessToken, DateTimeOffset accessExpires, _) = jwt.CreateAccessToken(user.Id, user.Username, sessionId);
        string newRawRefresh = jwt.GenerateRefreshTokenRaw();
        string newHash = jwt.HashRefreshToken(newRawRefresh);
        DateTimeOffset now = dateTime.UtcNow;
        DateTimeOffset refreshExpires = now.AddDays(14);
        RefreshToken replacement = new(user.Id, newHash, sessionId, now, refreshExpires);
        await refreshRepo.AddAsync(replacement, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        RefreshTokenDto dto = new(accessToken, accessExpires, newRawRefresh, refreshExpires, sessionId, user.Username);
        return dto;
    }
}
