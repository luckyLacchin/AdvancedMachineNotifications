namespace DemoAPIBot.Extensions
{
    public static class ApiAuthorizeExtensions
    {
        public static IApplicationBuilder UseApiAuthorize(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiUrlPortAuthMiddleware>();
        }
    }
}
