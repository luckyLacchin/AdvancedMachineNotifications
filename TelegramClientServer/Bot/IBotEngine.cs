using DemoServiceProto;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramClientServer.Models;

namespace TelegramClientServer.Bot
{
    public interface IBotEngine
    {
        public Task ListenForMessagesAsync();
        public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken);
        public Task SendNotification(Notification mex);

    }
}
