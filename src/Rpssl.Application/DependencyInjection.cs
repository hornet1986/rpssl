using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Rpssl.Application.Abstractions;
using Rpssl.Application.Behaviors;
using Rpssl.Application.Cqrs;
using Rpssl.Application.Services;

namespace Rpssl.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        System.Reflection.Assembly assembly = typeof(DependencyInjection).Assembly;

    services.AddSimpleCqrs(assembly);

        services.AddValidatorsFromAssembly(assembly);

        services.AddSingleton<IGameRulesEngine, GameRulesEngine>();

    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
