using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Rpssl.Application.Abstractions;
using Rpssl.SharedKernel;

namespace Rpssl.Infrastructure.Random;

internal sealed class RandomNumberService(HttpClient httpClient, IOptions<RandomNumberOptions> options, ILogger<RandomNumberService> logger) : IRandomNumberService
{
    private readonly RandomNumberOptions _options = options.Value;

    public async Task<Result<int>> GetRandomOneToFiveAsync(CancellationToken cancellationToken)
    {
        // Requirement: keep polling until the API returns a number in [1,5].
        // Retry on out-of-range values and on transient failures (HTTP/timeout/parse).
        AsyncRetryPolicy<int> policy = Policy<int>
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .Or<FormatException>()
            .OrResult(static r => r is < 1 or > 5)
            .WaitAndRetryAsync(
                retryCount: _options.MaxAttempts,
                sleepDurationProvider: _ => TimeSpan.FromMilliseconds(_options.DelayMs),
                onRetryAsync: async (outcome, delay, attempt, _) =>
                {
                    if (outcome.Exception is not null)
                    {
                        // Gracefully handle network/transient exceptions on each retry without flooding the console with stack traces
                        string reason = outcome.Exception.Message;
                        logger.LogInformation(
                            "Random number API attempt {Attempt}/{MaxAttempts} failed: {Reason}. Retrying in {Delay}.",
                            attempt, _options.MaxAttempts, reason, delay);
                    }
                    else
                    {
                        logger.LogWarning(
                            "Random number API returned out-of-range value {Value} on attempt {Attempt}/{MaxAttempts}. Retrying in {Delay}.",
                            outcome.Result, attempt, _options.MaxAttempts, delay);
                    }
                    await Task.CompletedTask;
                });

        try
        {
            int value = await policy.ExecuteAsync(async ct =>
            {
                using HttpRequestMessage request = new(HttpMethod.Get, BuildUri());
                using HttpResponseMessage response = await httpClient.SendAsync(request, ct);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync(ct);
                RandomNumberPayload? payload = JsonSerializer.Deserialize<RandomNumberPayload>(json) 
                                               ?? throw new FormatException("Random number API returned invalid JSON payload.");

                return payload.RandomNumber;
            }, cancellationToken);

            if (value is >= 1 and <= 5)
            {
                return Result.Success(value);
            }

            logger.LogError(
                "Random number API did not return a value within [1,5] after {MaxAttempts} attempts. Last value: {Value}. Uri: {Uri}",
                _options.MaxAttempts, value, BuildUri());
            return Result.Failure<int>(Error.Failure(
                code: "RandomNumber.OutOfRange",
                description: $"Random number API did not return a value within [1,5] after {_options.MaxAttempts} attempts."));

        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex,
                "Network failure contacting random number API after {MaxAttempts} attempts. Uri: {Uri}",
                _options.MaxAttempts, BuildUri());
            return Result.Failure<int>(Error.Problem(
                code: "RandomNumber.Network",
                description: $"Network failure contacting random number API after {_options.MaxAttempts} attempts: {ex.Message}"));
        }
        catch (TaskCanceledException ex)
        {
            logger.LogError(ex,
                "Random number API request timed out or was canceled after {MaxAttempts} attempts. Uri: {Uri}",
                _options.MaxAttempts, BuildUri());
            return Result.Failure<int>(Error.Problem(
                code: "RandomNumber.Timeout",
                description: $"Random number API request timed out or was canceled after {_options.MaxAttempts} attempts: {ex.Message}"));
        }
        catch (JsonException ex)
        {
            logger.LogError(ex,
                "Random number API returned malformed JSON after {MaxAttempts} attempts. Uri: {Uri}",
                _options.MaxAttempts, BuildUri());
            return Result.Failure<int>(Error.Problem(
                code: "RandomNumber.Json",
                description: $"Random number API returned malformed JSON after {_options.MaxAttempts} attempts: {ex.Message}"));
        }
        catch (FormatException ex)
        {
            logger.LogError(ex,
                "Random number API returned unexpected payload after {MaxAttempts} attempts. Uri: {Uri}",
                _options.MaxAttempts, BuildUri());
            return Result.Failure<int>(Error.Problem(
                code: "RandomNumber.Payload",
                description: $"Random number API returned unexpected payload after {_options.MaxAttempts} attempts: {ex.Message}"));
        }
    }

    private Uri BuildUri()
    {
        // Ensure single slash join
        string baseUrl = _options.BaseUrl.TrimEnd('/');
        string path = _options.EndpointPath.StartsWith('/') ? _options.EndpointPath : $"/{_options.EndpointPath}";
        return new Uri(baseUrl + path);
    }

    private sealed record RandomNumberPayload(
        [property: JsonPropertyName("random_number")] int RandomNumber);
}
