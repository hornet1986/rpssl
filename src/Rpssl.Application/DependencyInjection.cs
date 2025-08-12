using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Rpssl.Application.Abstractions;
using Rpssl.Application.Behaviors;
using Rpssl.Application.Services;

namespace Rpssl.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        System.Reflection.Assembly assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(configuration =>
                configuration.RegisterServicesFromAssembly(
                    typeof(DependencyInjection).Assembly));

        services.AddValidatorsFromAssembly(assembly);

        services.AddSingleton<IGameRulesEngine, GameRulesEngine>();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
