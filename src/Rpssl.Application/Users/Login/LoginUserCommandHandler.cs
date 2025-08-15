using Rpssl.Application.Abstractions;
using Rpssl.Domain.RefreshTokens;
using Rpssl.Domain.UnitOfWork;
using Rpssl.Domain.Users;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Users.Login;

internal sealed class LoginUserCommandHandler(
    IUserRepository users,
    IRefreshTokenRepository refreshTokens,
    IPasswordHasher hasher,
    IJwtTokenService jwt,
    IJwtOptionsAccessor jwtOptions,
    IDateTimeProvider dateTime,
    IUnitOfWork uow) : IRequestHandler<LoginUserCommand, Result<UserDto>>
{
    private static readonly Error InvalidCredentials = Error.Validation("Auth.InvalidCredentials", "Invalid credentials");

    public async Task<Result<UserDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        User? user = await users.GetByUsernameAsync(request.Username, cancellationToken);
        if (user is null || !hasher.Verify(request.Password, user.PasswordHash))
        {
            return Result.Failure<UserDto>(InvalidCredentials);
        }

        user.SetLastLogin(dateTime.UtcNow);
        string sessionId = Guid.NewGuid().ToString("N");

        (string accessToken, DateTimeOffset accessExpires, _) = jwt.CreateAccessToken(user.Id, user.Username, sessionId);
        string rawRefresh = jwt.GenerateRefreshTokenRaw();
        string refreshHash = jwt.HashRefreshToken(rawRefresh);
        DateTimeOffset now = dateTime.UtcNow;
        DateTimeOffset refreshExpires = now.AddDays(jwtOptions.RefreshTokenDays);
        RefreshToken rt = new(user.Id, refreshHash, sessionId, now, refreshExpires);
        await refreshTokens.AddAsync(rt, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        UserDto dto = new(accessToken, accessExpires, rawRefresh, refreshExpires, sessionId, user.Username);
        return dto;
    }
}
