using Microsoft.Extensions.Configuration;

namespace DemoAPIBot
{
    public static class EndpointParameters
    {
        public static void parseEndpoint(string endpoint, out bool secure, out int port) {
            port = int.Parse(endpoint.Split(":").Last());
            secure = endpoint.ToLower().Contains("https");
        }
    }
}
