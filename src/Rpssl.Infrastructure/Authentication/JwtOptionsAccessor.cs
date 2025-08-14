using Microsoft.Extensions.Options;
using Rpssl.Application.Abstractions;

namespace Rpssl.Infrastructure.Authentication;

internal sealed class JwtOptionsAccessor(IOptions<JwtOptions> options) : IJwtOptionsAccessor
{
    public int RefreshTokenDays => options.Value.RefreshTokenDays;
}
