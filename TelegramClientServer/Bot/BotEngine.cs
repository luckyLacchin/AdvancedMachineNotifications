using Azure.Core;
using DemoServiceProto;
using Grpc.Net.Client;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramClientServer.Data;
using TelegramClientServer.Models;

namespace TelegramClientServer.Bot
{
    public class BotEngine : BackgroundService, IBotEngine
    {
        private readonly TelegramBotClient _botClient;
        private readonly ILogger logger;
        TelegramContext db;
        private readonly ServerApi.ServerApiClient _client;

        public BotEngine(IConfiguration configuration, ILogger<BotEngine> _logger, ChannelServerApi _channelServerApi)
        {
            _botClient = new TelegramBotClient(configuration.GetValue<string>("TelegramCode"));
            logger = _logger;
            _client = _channelServerApi.client;
        }

        public async Task ListenForMessagesAsync()
        {
            using var cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };
            this._botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );
            try
            {
                var me = await _botClient.GetMeAsync();
                logger.LogInformation($"Start listening for @{me.Username}");
            }
            catch (Exception e)
            {

            }



        }

        /*
         * One thing to note in the above code is that StartReceiving 
         * does not block the caller thread as receiving is done 
         * on the ThreadPool.
        */

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (!IsAliveTimer.isConnected)
                return;
            // Only process Message updates
            if (update.Message is not { } message)
            {
                return;
            }

            // Only process text messages
            if (message.Text is not { } messageText)
            {
                return;
            }

            logger.LogInformation($"Received a '{messageText}' message in chat {message.Chat.Id}.");

            var chatId = message.Chat.Id;
            using (db = new TelegramContext()) //forse potrei usare questa istanza per tutti gli altri db, ma preferisco fare così per non avere possibile conflitti.
            {

                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[] {new KeyboardButton[]
            {
                "/sub",
                "/stop",
                "/unsub"
            },
            new KeyboardButton[]
            {
                "/getsubs",
                "/help",
                db.Admins.Where(x => x.chatId == chatId.ToString()).Any() ? "/getMessagesMId" : "",


            }
        });
                if (db.Admins.Where(x => x.chatId == chatId.ToString()).Any())
                {
                    if (ContainerRequest.getMessagesMid.Contains(chatId.ToString()) && messageText == "/stop")
                    {

                        ContainerRequest.getMessagesMid.Remove(chatId.ToString());
                        Notification mex = new Notification();
                        mex.chatId = chatId.ToString();
                        mex.message = "The procedure has been stopped";
                        await SendNotification(mex);
                    }
                    else if (ContainerRequest.getMessagesMid.Contains(chatId.ToString()))
                    {

                        Notification mex = new Notification();
                        mex.chatId = chatId.ToString();
                        using (db = new TelegramContext())
                        {
                            if (db.Chatters.Where(x => x.mId == messageText).Any())
                            {
                                GetMessagesMIdRequest getMessages = new GetMessagesMIdRequest();
                                getMessages.MId = messageText;
                                var _reply = await _client.GetMessagesMIdAsync(getMessages);
                                if (_reply.Outcome)
                                {
                                    mex.message = $"These are the notifications about this machine {messageText}: \n";
                                    foreach (MessageProto msg in _reply.Messages)
                                    {
                                        mex.message += $"[{msg.MId} - {msg.Date}] : {msg.Mex}\n";
                                    }
                                }
                                else
                                {
                                    mex.message = "There isn't any messages about these machine.";
                                }
                                ContainerRequest.getMessagesMid.Remove(chatId.ToString());

                            }
                            else
                            {
                                mex.message = "The machine's code written is not valid. Try again. \n Write /stop to cancel this procedure";
                            }
                        }
                        await SendNotification(mex);
                    }
                    else if (message.Text == "/getMessagesMId")
                    {
                        Notification mex = new Notification();
                        mex.chatId = chatId.ToString();
                        mex.message = "Write the code of the machine to get its messages from";
                        ContainerRequest.getMessagesMid.Add(chatId.ToString());
                        await SendNotification(mex);
                    }
                }
                if (message.Text == "/start")
                {
                    Telegram.Bot.Types.Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"Hi {message.Chat.FirstName} {message.Chat.LastName}, I'm BLMGroup Bot, ready to /help",
                    replyMarkup: replyKeyboardMarkup,
                    cancellationToken: cancellationToken);
                }
                else if (ContainerRequest.subscribeList.Contains(chatId.ToString()) && messageText == "/stop")
                    {
                        //questo è il caso in cui sbaglio token e voglio fermare la procedura. Dopo in serverApiDB, verrà cancellato automaticamente.
                        ContainerRequest.subscribeList.Remove(chatId.ToString());
                        Notification mex = new Notification();
                        mex.chatId = chatId.ToString();
                        mex.message = "The subscription's procedure has been stopped";
                        await SendNotification(mex);
                    }
                    else if (ContainerRequest.unsubscribeList.Contains(chatId.ToString()) && messageText == "/stop")
                    {
                        //non dovrebbe essere possibile che unsubscribeList e subscribeList contengano lo stesso ChatId
                        ContainerRequest.unsubscribeList.Remove(chatId.ToString());
                        Notification mex = new Notification();
                        mex.chatId = chatId.ToString();
                        mex.message = "The unsubscription's procedure has been stopped";
                        await SendNotification(mex);
                    }

                    else if (ContainerRequest.unsubscribeList.Contains(chatId.ToString()))
                    {
                        Notification mex = new Notification();
                        mex.chatId = chatId.ToString();
                        using (db = new TelegramContext())
                        {
                            DeleteSubRequest deleteSubRequest = new DeleteSubRequest();
                            deleteSubRequest.MId = messageText;
                            deleteSubRequest.Dispatcher = "Telegram";
                            deleteSubRequest.UsersId.AddRange(db.Chatters.Where(x => x.chatId == chatId.ToString()).Select(x => x.Id).ToList());
                            var _reply = await _client.DeleteSubAsync(deleteSubRequest);
                            if (_reply.Outcome)
                            {
                                mex.message = $"Subscription to machine {messageText} delete successfully";
                                ContainerRequest.unsubscribeList.Remove(chatId.ToString());
                                Chatter chatterToDelete = db.Chatters.Where(x => x.Id == _reply.UserId).FirstOrDefault(); // a questo punto è impossibile che sia == null!
                                db.Chatters.Remove(chatterToDelete);
                                await db.SaveChangesAsync();
                            }
                            else
                            {
                                mex.message = "The machine's code written is not valid. Try again. \n Write /stop to stop the subscription procedure";
                            }
                        }
                        await SendNotification(mex);
                    }
                    else if (ContainerRequest.subscribeList.Contains(chatId.ToString()))
                    {
                        TokenRequest tokenRequest = new TokenRequest { Token = messageText, Dispatcher = "Telegram" };
                        //a questo punto devo creare il channel Grpc inviare il token al server API sperando che vada tutto bene
                        var _reply = await _client.SendTokenAsync(tokenRequest);
                        Notification mex = new Notification();
                        mex.chatId = chatId.ToString();
                        if (_reply.Outcome)
                        {

                            using (db = new TelegramContext())
                            {
                                if (db.Chatters.Where(x => x.chatId == chatId.ToString() && x.mId == _reply.MId).Any())
                                {
                                    //vuol dire che sono già iscritto a questa machine
                                    mex.message = $"You are already subscribed to this machine: {_reply.MId}";
                                }
                                else
                                {
                                    Chatter newChatter = new Chatter(chatId.ToString(), "connected", _reply.MId);
                                    await db.AddAsync(newChatter);
                                    await db.SaveChangesAsync();
                                    int userId = newChatter.Id; //fa direttamente la retrieve dell'oggetto appena salvato.
                                    var reply = await _client.SendIdAsync(new SendIdRequest { UserId = userId, Token = messageText, Dispatcher = "Telegram" });
                                    if (reply.Outcome)
                                        logger.LogInformation("Subscription completed successfully");
                                    else //non dovrebbe capitare, caso in cui non trova il token, ma in realtà lo ha già trovato la prima volta
                                        logger.LogError("Something went wrong.");
                                    mex.message = "Subscription completed successfully";
                                }
                            }
                            ContainerRequest.subscribeList.Remove(chatId.ToString());

                        }
                        else
                        {
                            mex.message = "The token is wrong, try again. \n Write /stop to stop the subscription procedure";
                        }

                        //in realtà dovrei fare che venga tolto solo se ha fatto giusto, in caso avesse sbagliato riprova finché non mette un altro comando tipo: "indietro".
                        await SendNotification(mex);
                    }

                    else if (message.Text == "/sub")
                    {
                        Notification mex = new Notification();
                        mex.message = "Write the token";
                        mex.chatId = chatId.ToString();
                        ContainerRequest.subscribeList.Add(chatId.ToString());
                        await SendNotification(mex);
                    }
                    else if (message.Text == "/getsubs")
                    {
                        logger.LogInformation("getSubs Text");
                        GetSubsRequest request = new GetSubsRequest();
                        using (db = new TelegramContext())
                        {
                            var List = db.Chatters.Where<Chatter>(x => x.chatId == chatId.ToString()).Select(x => x.Id).ToList();
                            Notification mex = new Notification();
                            mex.chatId = chatId.ToString();

                            if (List.Count > 0)
                            {
                                request.UserId.AddRange(List);
                                var reply = await _client.GetSubsAsync(request);
                                mex.message = "You are currently subscribed to the following machines: \n";
                                foreach (string mId in reply.MIds)
                                {
                                    mex.message += $"{mId}, "; //in futuro magari sarebbe meglio avere dei codici migliore per le macchine, mettendo anche delle stringhe
                                }
                            }
                            else
                            {
                                //in questo caso l'user non è iscritto a nessuna macchina. Gli scrivo un messaggio per avvisarlo di ciò
                                mex.message = "You are not subscrite to any machine.";
                            }
                        logger.LogInformation("After getSubs Text");
                        await SendNotification(mex);
                        }
                    }

                    else if (message.Text == "/unsub")
                    {
                        using (db = new TelegramContext())
                        {
                            Notification mex = new Notification();
                            mex.chatId = chatId.ToString();
                            if (db.Chatters.Where(x => x.chatId == chatId.ToString()) != null)
                            {
                                mex.message = "Write the machine's code to unsubscribe from";
                                ContainerRequest.unsubscribeList.Add(chatId.ToString());
                                await SendNotification(mex);
                            }
                            else
                            {
                                mex.message = "You are not subscribed to any machines";
                            }
                        }

                    }
                    else if (message.Text == "/help")
                    {
                        Notification mex = new Notification();
                        mex.chatId = chatId.ToString();
                        mex.message = "This bot allows to receive all the notifications regards the BLM group's machines you'are registered to. \n\n" +
                            "These are the commands that are available: \n " +
                            "/sub: Begin the subscription's procedure \n" +
                            "/stop: Stop the procedure you are doing \n" +
                            "/getsubs: Get the machines you are subscribed to \n" +
                            "/unsub: Unsubscribe from a machine";
                        await SendNotification(mex);
                    }

                
            }

        }

        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        public async Task SendNotification(Notification mex)
        {
            using var cts = new CancellationTokenSource();
            Telegram.Bot.Types.Message message = await _botClient.SendTextMessageAsync(
            chatId: mex.chatId,
            text: mex.message,
            cancellationToken: cts.Token);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return ListenForMessagesAsync();
        }
    }
}
