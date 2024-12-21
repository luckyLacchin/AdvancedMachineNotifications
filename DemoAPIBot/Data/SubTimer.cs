using CommunityToolkit.Mvvm.Messaging;
using DemoAPIBot.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoAPIBot.Data
{
    public class SubTimer : BackgroundService
    {
        private DemoContext db;
        public bool _ready = false;
        public SubTimer(ILogger<SubTimer> _logger, IHostApplicationLifetime lifetime)
        {
            Logger = _logger;
            Timer = new System.Timers.Timer(5_000);
            lifetime.ApplicationStarted.Register(() => _ready = true);
        }

        private System.Timers.Timer Timer { get; set; }
        private ILogger Logger { get; set; }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            Timer.Elapsed += (s, e) => {
                if (_ready)
                {
                    db = new DemoContext();
                    DateTime scadenza = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(1));
                    TimeSpan minute = TimeSpan.FromMinutes(5);
                    List<InProgressSub> toDelete = db.InProgressSubs.Where<InProgressSub>(x => scadenza > x.timeSubRequest).ToList();
                    foreach (InProgressSub x in toDelete)
                    {
                        Logger.LogTrace($"Rimosso: {x.mId}");
                        db.InProgressSubs.Remove(x);
                        db.SaveChanges(); //forse qua potremmo avere un problema di conflitto nel caso in cui stessimo facendo qualcos'altro con il db da qualche altra parte

                    }
                    
                }
            };
            Timer.Start();
            Timer.AutoReset = true;
            return Task.CompletedTask;
        }
    }
}
