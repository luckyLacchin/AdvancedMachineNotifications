using DemoAPIBot.Messanger;
using DemoAPIBot.Models;
using DemoAPIBot.ServiceDispatchers;
using DemoServiceProto;
using System.Linq;

namespace DemoAPIBot.Data
{
    public class PingTimer : BackgroundService
    {
        private DemoContext db;
        public bool _ready = false;
        private System.Timers.Timer Timer { get; set; }
        private ILogger Logger { get; set; }
        public PingTimer(ILogger<PingTimer> _logger, IHostApplicationLifetime lifetime)
        {
            Logger = _logger;
            Timer = new System.Timers.Timer(2_000);
            lifetime.ApplicationStarted.Register(() => _ready = true);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Timer.Elapsed += async (s, e) => {
                if (_ready)
                {
                    for (int i = 0; i < DispatchersContainer.DispatchersList.Count; i++)
                    {
                        ServiceDispatcher service = DispatchersContainer.DispatchersList[i];
                        try
                        {
                            var reply = await service.client.IsDispatcherAliveAsync(new IsDispatcherAliveMessage());
                            Logger.LogTrace($"Dispatcher {service.Name} is still alive");
                        }
                        catch (Exception ex)
                        {
                            Logger.LogTrace(ex, $"Dispatcher {service.Name} isn't alive");
                            DispatchersContainer.deleteDispatcher(service);
                        }
                    }
                }
            };
            Timer.Start();
            Timer.AutoReset = true;
            return Task.CompletedTask;
        }
    }
}
