using Rpssl.Application.Abstractions;
using Rpssl.Application.Token;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Users.Login;

public sealed record LoginUserCommand(string Username, string Password) : IRequest<Result<UserDto>>;
