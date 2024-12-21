using DemoServiceProto;
using Grpc.Net.Client;
using System.Runtime.InteropServices;

namespace TelegramClientServer.Data
{
    public class ChannelServerApi
    {
        SocketsHttpHandler handler = new SocketsHttpHandler
        {
            PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
            KeepAlivePingDelay = TimeSpan.FromSeconds(60),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
            EnableMultipleHttp2Connections = true,
            ConnectTimeout = TimeSpan.FromSeconds(60),
        };

        public string apiAddress;
        public GrpcChannel channel;
        public ServerApi.ServerApiClient client;
        ILogger<ChannelServerApi> _logger;
        public ChannelServerApi(IConfiguration configuration, ILogger <ChannelServerApi> logger)
        {
            apiAddress = $"{(configuration.GetValue<bool>("ServerApiSecure")?"https":"http")}://{configuration.GetValue<string>("ServerApiAddress")}:{configuration.GetValue<int>("ServerApiPort")}";
            channel = GrpcChannel.ForAddress(apiAddress, new GrpcChannelOptions
            {
                HttpHandler = handler
            });
            client = new ServerApi.ServerApiClient(channel);
            _logger = logger;
            _logger.LogInformation($"Constructor ChannelServerApi {apiAddress}");
        }
    }
}
