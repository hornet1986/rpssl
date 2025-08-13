using Rpssl.Application.Abstractions;

namespace Rpssl.Application.Cqrs;

internal sealed class ServiceProviderSender(IServiceProvider serviceProvider) : ISender
{
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        // Resolve handler
        Type handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        dynamic? handler = serviceProvider.GetService(handlerType);
        if (handler is null)
        {
            throw new InvalidOperationException($"No handler registered for {request.GetType().Name}");
        }

        // Build pipeline
        IEnumerable<object> behaviors = GetBehaviors<TResponse>(request);

        RequestHandlerDelegate<TResponse> terminal = ct => handler.Handle((dynamic)request, ct);

        // Compose behaviors in reverse order
        foreach (object behaviorObj in behaviors.Reverse())
        {
            dynamic behavior = (dynamic)behaviorObj;
            RequestHandlerDelegate<TResponse> nextCopy = terminal;
            terminal = ct => behavior.Handle((dynamic)request, nextCopy, ct);
        }

        return await terminal(cancellationToken);
    }

    private IEnumerable<object> GetBehaviors<TResponse>(object request)
    {
        Type behaviorOpenGeneric = typeof(IPipelineBehavior<,>);
        Type behaviorType = behaviorOpenGeneric.MakeGenericType(request.GetType(), typeof(TResponse));
        // Resolve IEnumerable<IPipelineBehavior<TRequest, TResponse>> using non-generic service resolution
        Type enumerableType = typeof(IEnumerable<>).MakeGenericType(behaviorType);
        object? resolved = serviceProvider.GetService(enumerableType);
        return resolved as IEnumerable<object> ?? Array.Empty<object>();
    }
}
