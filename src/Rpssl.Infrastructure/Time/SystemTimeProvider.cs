using Rpssl.Application.Abstractions;

namespace Rpssl.Infrastructure.Time;

internal sealed class SystemTimeProvider : ITimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
