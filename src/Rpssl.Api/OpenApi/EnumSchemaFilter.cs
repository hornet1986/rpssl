using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Rpssl.Api.OpenApi;

/// <summary>
/// Forces enum schemas to display as string names instead of underlying numeric values in Swagger UI.
/// </summary>
public sealed class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum)
        {
            return;
        }

        schema.Enum = context.Type
            .GetEnumNames()
            .Select(IOpenApiAny (n) => new OpenApiString(n))
            .ToList();
        schema.Type = "string";
        schema.Format = null;
    }
}
