namespace Rpssl.Application.Abstractions;

public interface ITimeProvider
{
    /// <summary>
    /// Gets the current time in Coordinated Universal Time (UTC).
    /// </summary>
    DateTimeOffset UtcNow { get; }
}
