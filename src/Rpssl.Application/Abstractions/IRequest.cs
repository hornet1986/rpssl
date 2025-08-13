namespace Rpssl.Application.Abstractions;

/// <summary>
/// Marker interface representing a request message that produces a <typeparamref name="TResponse"/>.
/// Used to associate handlers and pipeline behaviors with a specific message type.
/// </summary>
/// <typeparam name="TResponse">The type returned when the request is processed.</typeparam>
public interface IRequest<TResponse> { }
