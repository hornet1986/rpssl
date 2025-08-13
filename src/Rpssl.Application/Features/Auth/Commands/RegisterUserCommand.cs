using Rpssl.Application.Abstractions;
using Rpssl.Domain.Entities;
using Rpssl.Domain.Repositories;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Features.Auth.Commands;

public sealed record RegisterUserCommand(string Username, string Password) : IRequest<Result<AuthResultDto>>;

internal sealed class RegisterUserCommandHandler(
    IUserRepository users,
    IRefreshTokenRepository refreshTokens,
    IPasswordHasher hasher,
    IJwtTokenService jwt,
    ITimeProvider time,
    IUnitOfWork uow) : IRequestHandler<RegisterUserCommand, Result<AuthResultDto>>
{
    private static readonly Error UsernameTaken = Error.Conflict("Auth.UsernameTaken", "Username already in use");

    public async Task<Result<AuthResultDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        User? existing = await users.GetByUsernameAsync(request.Username, cancellationToken);
        if (existing is not null)
        {
            return Result.Failure<AuthResultDto>(UsernameTaken);
        }

        string passwordHash = hasher.Hash(request.Password);
        User user = new(request.Username, passwordHash, Guid.NewGuid().ToString("N"));
        await users.AddAsync(user, cancellationToken);

        string sessionId = Guid.NewGuid().ToString("N");
        (string accessToken, DateTimeOffset accessExpires, _) = jwt.CreateAccessToken(user.Id, user.Username, sessionId);
        string rawRefresh = jwt.GenerateRefreshTokenRaw();
        string refreshHash = jwt.HashRefreshToken(rawRefresh);
        DateTimeOffset now = time.UtcNow;
        DateTimeOffset refreshExpires = now.AddDays(14); // TODO: inject from options for consistency
        RefreshToken rt = new(user.Id, refreshHash, sessionId, now, refreshExpires);
        await refreshTokens.AddAsync(rt, cancellationToken);

        await uow.SaveChangesAsync(cancellationToken);

        AuthResultDto dto = new(accessToken, accessExpires, rawRefresh, refreshExpires, sessionId, user.Username);
        return Result.Success(dto);
    }
}
