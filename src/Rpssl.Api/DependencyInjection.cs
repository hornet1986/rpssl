using Rpssl.Api.Extensions;
using Rpssl.Api.Infrastructure;

namespace Rpssl.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddControllers();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.ConfigureApiVersioning();

        return services;
    }
}
