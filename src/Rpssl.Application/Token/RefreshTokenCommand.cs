using Rpssl.Application.Abstractions;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Token;

public sealed record RefreshTokenCommand(string RefreshToken, string SessionId) : IRequest<Result<RefreshTokenDto>>;
