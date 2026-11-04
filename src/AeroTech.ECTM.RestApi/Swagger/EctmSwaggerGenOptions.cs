using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AeroTech.ECTM.RestApi.Swagger
{
    public sealed class EctmSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
    {
        public const string Title = "AeroTech ECTM Backoffice Domain API";
        public const string IdentifierPattern = "^[0-9]+$";

        private const string DocumentName = "v1";
        private const string Version = "3.3.0";
        private const string Description = "ECTM v3.3 backoffice API using the AeroTech framework conventions.";
        private const string IdentifierDescription = "Framework Snowflake long JSON string.";
        private const string ControllerNamespacePrefix = "AeroTech.ECTM.RestApi.V1.";
        private const string AggregateSuffix = "Aggregate";

        public void Configure(SwaggerGenOptions options)
        {
            options.SwaggerDoc(DocumentName, new OpenApiInfo
            {
                Title = Title,
                Version = Version,
                Description = Description
            });

            options.MapType<long>(() => Identifier(JsonSchemaType.String));
            options.MapType<long?>(() => Identifier(JsonSchemaType.String | JsonSchemaType.Null));
            options.ParameterFilter<IdentifierParameterFilter>();
            options.TagActionsBy(api => [TagOf(api)]);
            options.OrderActionsBy(api => $"{TagOf(api)}_{api.RelativePath}_{api.HttpMethod}");
            options.ResolveConflictingActions(actions => actions.First());
        }

        private static OpenApiSchema Identifier(JsonSchemaType type)
            => new()
            {
                Type = type,
                Pattern = IdentifierPattern,
                Description = IdentifierDescription
            };

        private static string TagOf(ApiDescription api)
        {
            var controllerNamespace = (api.ActionDescriptor as ControllerActionDescriptor)?.ControllerTypeInfo.Namespace ?? string.Empty;

            if (!controllerNamespace.StartsWith(ControllerNamespacePrefix, StringComparison.Ordinal))
                return controllerNamespace;

            var aggregate = controllerNamespace[ControllerNamespacePrefix.Length..].Split('.')[0];

            return aggregate.EndsWith(AggregateSuffix, StringComparison.Ordinal) ? aggregate[..^AggregateSuffix.Length] : aggregate;
        }
    }
}
