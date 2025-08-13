namespace Rpssl.Application.Abstractions;

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>(CancellationToken cancellationToken);

/// <summary>
/// Defines a pipeline behavior (middleware) that can surround the execution of a request handler.
/// Typical behaviors include validation, logging, performance metrics, transactions, retries, authorization etc.
/// </summary>
/// <typeparam name="TRequest">The request (message) type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface IPipelineBehavior<in TRequest, TResponse>
{
    /// <summary>
    /// Handles the request either by performing work before/after invoking the next delegate
    /// or short-circuiting and returning a response directly.
    /// </summary>
    /// <param name="request">The incoming request instance.</param>
    /// <param name="next">The continuation delegate to invoke the rest of the pipeline / final handler.</param>
    /// <param name="cancellationToken">Token to observe cancellation.</param>
    /// <returns>The response (from this behavior or the downstream handler).</returns>
    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}
