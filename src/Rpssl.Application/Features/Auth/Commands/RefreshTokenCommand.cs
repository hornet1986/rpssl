using Rpssl.Application.Abstractions;
using Rpssl.Domain.Entities;
using Rpssl.Domain.Repositories;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Features.Auth.Commands;

public sealed record RefreshTokenCommand(string RefreshToken, string SessionId) : IRequest<Result<AuthResultDto>>;

internal sealed class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshRepo,
    IUserRepository users,
    IJwtTokenService jwt,
    ITimeProvider time,
    IUnitOfWork uow) : IRequestHandler<RefreshTokenCommand, Result<AuthResultDto>>
{
    private static readonly Error InvalidRefresh = Error.Validation("Auth.InvalidRefresh", "Invalid or expired refresh token");

    public async Task<Result<AuthResultDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        string hash = jwt.HashRefreshToken(request.RefreshToken);
        RefreshToken? existing = await refreshRepo.GetByHashAsync(hash, cancellationToken);
        if (existing is null || !existing.IsActive(time.UtcNow) || existing.SessionId != request.SessionId)
        {
            return Result.Failure<AuthResultDto>(InvalidRefresh);
        }

        User? user = await users.GetByIdAsync(existing.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<AuthResultDto>(InvalidRefresh);
        }

        // Rotate
        existing.Revoke(time.UtcNow);
        string sessionId = existing.SessionId; // same session
        (string accessToken, DateTimeOffset accessExpires, _) = jwt.CreateAccessToken(user.Id, user.Username, sessionId);
        string newRawRefresh = jwt.GenerateRefreshTokenRaw();
        string newHash = jwt.HashRefreshToken(newRawRefresh);
        DateTimeOffset now = time.UtcNow;
        DateTimeOffset refreshExpires = now.AddDays(14);
        RefreshToken replacement = new(user.Id, newHash, sessionId, now, refreshExpires);
        await refreshRepo.AddAsync(replacement, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        AuthResultDto dto = new(accessToken, accessExpires, newRawRefresh, refreshExpires, sessionId, user.Username);
        return Result.Success(dto);
    }
}
