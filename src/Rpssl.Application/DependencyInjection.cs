﻿using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Rpssl.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(configuration => 
            configuration.RegisterServicesFromAssembly(
                typeof(DependencyInjection).Assembly));

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}