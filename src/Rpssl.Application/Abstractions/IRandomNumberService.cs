using Rpssl.SharedKernel;

namespace Rpssl.Application.Abstractions;

public interface IRandomNumberService
{
    /// <summary>
    /// Polls the remote random number endpoint until a value in the range [1,5] is returned.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result with an integer between 1 and 5 inclusive, or an error describing the failure.</returns>
    Task<Result<int>> GetRandomOneToFiveAsync(CancellationToken cancellationToken);
}
