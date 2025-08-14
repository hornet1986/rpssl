namespace Rpssl.Application.Abstractions;

public interface IDateTimeProvider
{
    /// <summary>
    /// Gets the current time in Coordinated Universal Time (UTC).
    /// </summary>
    DateTimeOffset UtcNow { get; }
}
