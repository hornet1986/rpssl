namespace Rpssl.Application.Abstractions;

public interface ISender
{
    /// <summary>
    /// Dispatches a request to its handler, passing through any registered pipeline behaviors.
    /// </summary>
    /// <typeparam name="TResponse">The expected response type.</typeparam>
    /// <param name="request">The request instance to process.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The response produced by the handler.</returns>
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}
