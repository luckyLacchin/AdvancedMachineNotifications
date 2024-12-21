namespace DemoAPIBot.Extensions
{
    public static class SwaggerAuthorizeExtensions
    {
        public static IApplicationBuilder UseSwaggerAuthorize(this IApplicationBuilder builder) //with "this" I say that I am extending the type IApplicationBuilder
        {
            return builder.UseMiddleware<SwaggerUrlPortAuthMiddleware>();
        }
    }
}
