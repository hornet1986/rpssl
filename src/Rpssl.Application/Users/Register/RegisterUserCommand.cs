using Rpssl.Application.Abstractions;
using Rpssl.Application.Token;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Users.Register;

public sealed record RegisterUserCommand(string Username, string Password) : IRequest<Result<UserDto>>;
