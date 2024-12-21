using Grpc.Core;
using DemoServiceProto;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Logging;
using TelegramClientServer.Data;
using TelegramClientServer.Models;
using TelegramClientServer.Bot;
/*
* dovrei trovare il modo di mettere NLog come logger,
* se non ce la faccio forse a questo punto non ha senso mettere
* questa libreria. Anche se è molto fare qualcosa che sia valido
* per tutti i dispatchers.
* */

namespace TelegramClientServer.Services
{
    public class DispatcherService : Dispatcher.DispatcherBase
    {
        private readonly ILogger<DispatcherService> _logger;
        private readonly TelegramContext db;
        private readonly IBotEngine _bot;
        public DispatcherService(ILogger<DispatcherService> logger, TelegramContext dbContext, IBotEngine bot)
        {
            _logger = logger;
            db = dbContext;
            _bot = bot;
        }

        public override Task<IsDispatcherAliveMessage> IsDispatcherAlive(IsDispatcherAliveMessage request, ServerCallContext context)
        {
            _logger.LogError("Just enter IsDispatcherAlive");
            return Task.FromResult(new IsDispatcherAliveMessage());
        }

        public override Task<MsgResponse> SendMsg(Msg request, ServerCallContext context)
        {
            try
            {
                Chatter receiving = db.Chatters.Where(chatter => chatter.Id == request.UserId).SingleOrDefault();
                if (receiving == null)
                {
                    //non so se possa essere null in questo caso ma credo che abbiamo già gestito il fatto
                    //che l'utente non possa esistere in serverAPI.
                    return Task.FromResult(new MsgResponse
                    {
                        Response = false
                    });
                }
                else
                {
                    Notification notification = new Notification() { message = $"[{request.MId} - {request.DateTime}] : {request.SentMessage}", chatId = receiving.chatId };
                    _bot.SendNotification(notification);
                    return Task.FromResult(new MsgResponse
                    {
                        Response = true
                    });
                }
            } catch (Exception ex)
            {
                //nel caso in cui il ricevente è più di uno è un problema --> errore
                _logger.LogError(ex.Message);
                return Task.FromResult(new MsgResponse
                {
                    Response = false
                });
            }
        }
    }
}
