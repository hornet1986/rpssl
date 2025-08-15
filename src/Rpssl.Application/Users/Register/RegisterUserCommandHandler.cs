using Rpssl.Application.Abstractions;
using Rpssl.Domain.RefreshTokens;
using Rpssl.Domain.UnitOfWork;
using Rpssl.Domain.Users;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Users.Register;

internal sealed class RegisterUserCommandHandler(
    IUserRepository users,
    IRefreshTokenRepository refreshTokens,
    IPasswordHasher hasher,
    IJwtTokenService jwt,
    IJwtOptionsAccessor jwtOptions,
    IDateTimeProvider dateTime,
    IUnitOfWork uow) : IRequestHandler<RegisterUserCommand, Result<UserDto>>
{
    private static readonly Error UsernameTaken = Error.Conflict("Auth.UsernameTaken", "Username already in use");

    public async Task<Result<UserDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        User? existing = await users.GetByUsernameAsync(request.Username, cancellationToken);
        if (existing is not null)
        {
            return Result.Failure<UserDto>(UsernameTaken);
        }

        string passwordHash = hasher.Hash(request.Password);
        User user = new(request.Username, passwordHash, Guid.NewGuid().ToString("N"));

        user.Raise(new UserRegisteredDomainEvent(user.Id));
        await users.AddAsync(user, cancellationToken);

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
