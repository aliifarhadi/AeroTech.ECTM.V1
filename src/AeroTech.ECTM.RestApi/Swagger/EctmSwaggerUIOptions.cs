using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace AeroTech.ECTM.RestApi.Swagger
{
    public sealed class EctmSwaggerUIOptions : IConfigureOptions<SwaggerUIOptions>
    {
        public void Configure(SwaggerUIOptions options)
        {
            options.DocumentTitle = EctmSwaggerGenOptions.Title;
            options.DisplayRequestDuration();
        }
    }
}
