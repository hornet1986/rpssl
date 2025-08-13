using Rpssl.Application.Abstractions;
using Rpssl.Domain.Entities;
using Rpssl.Domain.Repositories;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Features.Auth.Commands;

public sealed record LoginUserCommand(string Username, string Password) : IRequest<Result<AuthResultDto>>;

internal sealed class LoginUserCommandHandler(
    IUserRepository users,
    IRefreshTokenRepository refreshTokens,
    IPasswordHasher hasher,
    IJwtTokenService jwt,
    ITimeProvider time,
    IUnitOfWork uow) : IRequestHandler<LoginUserCommand, Result<AuthResultDto>>
{
    private static readonly Error InvalidCredentials = Error.Validation("Auth.InvalidCredentials", "Invalid credentials");

    public async Task<Result<AuthResultDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        User? user = await users.GetByUsernameAsync(request.Username, cancellationToken);
        if (user is null || !hasher.Verify(request.Password, user.PasswordHash))
        {
            return Result.Failure<AuthResultDto>(InvalidCredentials);
        }

        user.SetLastLogin(time.UtcNow);
        string sessionId = Guid.NewGuid().ToString("N");

        (string accessToken, DateTimeOffset accessExpires, _) = jwt.CreateAccessToken(user.Id, user.Username, sessionId);
        string rawRefresh = jwt.GenerateRefreshTokenRaw();
        string refreshHash = jwt.HashRefreshToken(rawRefresh);
        DateTimeOffset now = time.UtcNow;
        DateTimeOffset refreshExpires = now.AddDays(14);
        RefreshToken rt = new(user.Id, refreshHash, sessionId, now, refreshExpires);
        await refreshTokens.AddAsync(rt, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        AuthResultDto dto = new(accessToken, accessExpires, rawRefresh, refreshExpires, sessionId, user.Username);
        return Result.Success(dto);
    }
}
