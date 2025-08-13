using System.Text.Json.Serialization;
using Rpssl.Api.Extensions;
using Rpssl.Api.Infrastructure;
using Rpssl.Api.OpenApi;

namespace Rpssl.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            c.SchemaFilter<EnumSchemaFilter>();
        });
        services.AddControllers()
            .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.ConfigureApiVersioning();

        return services;
    }
}
