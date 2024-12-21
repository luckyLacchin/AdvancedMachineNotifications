using CommunityToolkit.Mvvm.Messaging;
using TelegramClientServer.Models;
using Microsoft.EntityFrameworkCore;
using DemoServiceProto;

namespace TelegramClientServer.Data
{
    public class IsAliveTimer : BackgroundService
    {
        public bool _ready = false;
        private readonly ServerApi.ServerApiClient _client;
        private System.Timers.Timer Timer { get; set; }
        private ILogger Logger { get; set; }
        private readonly IConfiguration configuration;
        public static bool isConnected = false;
        public IsAliveTimer(ILogger<IsAliveTimer> _logger, IHostApplicationLifetime lifetime, ChannelServerApi channelServerApi, IConfiguration _configuration)
        {
            Logger = _logger;
            Timer = new System.Timers.Timer(2_000);
            _client = channelServerApi.client;
            configuration = _configuration;
            lifetime.ApplicationStarted.Register(() => _ready = true);
            
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.LogTrace("Start ExecuteAsync IsAliveTimer");
            Timer.Elapsed += async (s, e) => {
                if (_ready && !isConnected)
                {
                    var _input = new ServerAlive();
                    _input.Name = "Telegram";
                    var url = configuration.GetValue<string>("Kestrel:Endpoints:Grpc:Url");
                    _input.Port = int.Parse(url.Split(":").Last()); 
                    _input.Ip = configuration.GetValue<string>("GrpcApiAddress");
                    _input.Https = url.ToLower().Contains("https");
                    try
                    {
                        Logger.LogTrace("Send isAlive");
                        var _reply = await _client.IsServerAliveAsync(_input);
                        isConnected = true;
                        Logger.LogInformation(_reply.Message);
                        Timer.Stop();
                        Logger.LogInformation("Connection Telegram created successfully");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex,ex.Message);
                    }
                }
            };
            Timer.Start();
            Logger.LogWarning("Start Timer IsAliveTimer");
            Timer.AutoReset = true;
            return Task.CompletedTask;
        }
    }
}
