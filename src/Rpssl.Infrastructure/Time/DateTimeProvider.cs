using Rpssl.Application.Abstractions;

namespace Rpssl.Infrastructure.Time;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
