using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rpssl.Application.Abstractions;

namespace Rpssl.Application.Cqrs;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSimpleCqrs(this IServiceCollection services, Assembly assembly)
    {
        // Register sender
        services.AddScoped<ISender, ServiceProviderSender>();

        // Register all request handlers
        Type handlerInterface = typeof(IRequestHandler<,>);
        foreach (Type type in assembly.GetTypes())
        {
            if (type.IsAbstract || type.IsInterface)
            {
                continue;
            }

            var implemented = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterface)
                .ToList();

            foreach (Type iface in implemented)
            {
                services.AddTransient(iface, type);
            }
        }

        return services;
    }
}
