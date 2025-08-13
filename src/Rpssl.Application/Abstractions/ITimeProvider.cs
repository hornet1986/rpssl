namespace Rpssl.Application.Abstractions;

public interface ITimeProvider
{
    DateTimeOffset UtcNow { get; }
}
