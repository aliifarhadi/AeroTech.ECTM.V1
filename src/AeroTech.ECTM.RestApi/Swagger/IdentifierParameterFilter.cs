using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AeroTech.ECTM.RestApi.Swagger
{
    public sealed class IdentifierParameterFilter : IParameterFilter
    {
        public void Apply(IOpenApiParameter parameter, ParameterFilterContext context)
        {
            var type = context.ApiParameterDescription.Type;

            if (type != typeof(long) && type != typeof(long?))
                return;

            if (parameter.Schema is not OpenApiSchema schema)
                return;

            schema.Type = type == typeof(long) ? JsonSchemaType.String : JsonSchemaType.String | JsonSchemaType.Null;
            schema.Format = null;
            schema.Pattern = EctmSwaggerGenOptions.IdentifierPattern;
        }
    }
}
