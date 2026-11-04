using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace AeroTech.ECTM.RestApi.Middlewares
{
    public sealed class CorrelationIdMiddleware
    {
        public const string HeaderName = "X-Correlation-Id";

        private const string ActivityName = "AeroTech.ECTM.Request";

        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            var startedHere = Activity.Current is null;
            var activity = Activity.Current ?? new Activity(ActivityName).Start();

            context.Response.Headers[HeaderName] = activity.TraceId.ToHexString();

            try
            {
                await _next(context);
            }
            finally
            {
                if (startedHere)
                    activity.Stop();
            }
        }
    }
}
