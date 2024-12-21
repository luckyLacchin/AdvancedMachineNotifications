using DemoAPIBot.Models;
using System.Linq;

namespace DemoAPIBot.Data
{
    public class RefreshTokenTimer : BackgroundService
    {
        private DemoContext db;
        public bool _ready = false;
        public RefreshTokenTimer(ILogger<RefreshTokenTimer> _logger, IHostApplicationLifetime lifetime)
        {
            Logger = _logger;
            Timer = new System.Timers.Timer(10_000); //facciamo ogni 10s, non so se sia un tempo adeguato: https://github.com/FastEndpoints/Refresh-Tokens-Demo/blob/main/Features/User/Auth/RefreshToken/RefreshToken.cs 
            lifetime.ApplicationStarted.Register(() => _ready = true);
        }

        private System.Timers.Timer Timer { get; set; }
        private ILogger<RefreshTokenTimer> Logger { get; set; }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            Timer.Elapsed += (s, e) => {
                if (_ready)
                {
                    db = new DemoContext();
                    List<RefreshToken> toDelete = db.RefreshTokens.Where(x => DateTime.UtcNow > x.ExpiryDate).ToList();
                    foreach (RefreshToken x in toDelete)
                    {
                        Logger.LogTrace($"Rimosso: {x.UserID}");
                        db.RefreshTokens.Remove(x);
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
