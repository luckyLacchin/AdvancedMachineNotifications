using System.Net;

namespace DemoAPIBot.Extensions
{
    public class ApiUrlPortAuthMiddleware
    {
        private readonly RequestDelegate next;
        private ILogger logger;

        public ApiUrlPortAuthMiddleware(RequestDelegate next, ILogger<ApiUrlPortAuthMiddleware> _logger)
        {
            logger = _logger;
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
        {
            logger.LogInformation($"Arrivata richiesta da {context.Request.Host} porta locale: {context.Connection.LocalPort}");
            EndpointParameters.parseEndpoint(configuration.GetValue<string>("Kestrel:Endpoints:ApiEndpoint:Url"), out bool https, out int apiPort);
            //Make sure we are hitting the swagger path, and not doing it locally and are on the management port
            if (context.Request.Path.StartsWithSegments("/api") && context.Connection.LocalPort != apiPort)
            {
                logger.LogWarning("Richiesta non valida, Unauthorized");
                // Return unauthorized
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else
            {
                await next.Invoke(context);
            }
        }
    }
}
