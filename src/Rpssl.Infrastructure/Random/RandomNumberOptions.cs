namespace Rpssl.Infrastructure.Random;

public sealed class RandomNumberOptions
{
    public const string SectionName = "RandomNumber";

    public string BaseUrl { get; init; } = string.Empty;

    public string EndpointPath { get; init; } = "/random";

    /// <summary>
    /// Maximum number of attempts to poll until a value 1-5 is returned.
    /// </summary>
    public int MaxAttempts { get; init; } = 20;

    /// <summary>
    /// Delay between attempts in milliseconds.
    /// </summary>
    public int DelayMs { get; init; } = 100;
}
