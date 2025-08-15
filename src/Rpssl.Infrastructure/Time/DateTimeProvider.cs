using Rpssl.Application.Abstractions;
using Rpssl.SharedKernel;

namespace Rpssl.Infrastructure.Time;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
