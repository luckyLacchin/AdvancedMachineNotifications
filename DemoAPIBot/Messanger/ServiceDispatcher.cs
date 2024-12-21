using CommunityToolkit.Mvvm.ComponentModel;
using Grpc.Net.Client;
using System.Threading.Channels;
using CommunityToolkit.Mvvm.Messaging;
using NLog.LayoutRenderers;
using DemoServiceProto;
using DemoAPIBot.Data;
using Microsoft.AspNetCore.Mvc;

namespace DemoAPIBot.Messanger
{
    public class ServiceDispatcher : ObservableRecipient, IServiceDispatcher
    {
        static SocketsHttpHandler handler = new SocketsHttpHandler
        {
            PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
            KeepAlivePingDelay = TimeSpan.FromSeconds(60),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
            EnableMultipleHttp2Connections = true
        };
        public string Ip { get; private set; }
        public int Port { get; private set; }
        public string Name { get; private set; }
        public bool Https { get; private set; }
        private GrpcChannel channel;
        public Dispatcher.DispatcherClient client;
        //private readonly ILogger<ServiceDispatcher> logger; Non riesco ad ottenerlo con una DI..

        public ServiceDispatcher(string name)
        {
            Name = name;
        }

        public ServiceDispatcher()
        {
        }
        public void SetService(ServerAlive request)
        {
            Ip = request.Ip;
            Port = request.Port;
            Name = request.Name;
            Https = request.Https;
            if (CreateChannel())
            {
                Console.WriteLine("Channel in DemoAPI created successfully");
                Console.WriteLine($"Ip dispatcher: { request.Ip} , port : {request.Port} , name: {request.Name} ");
                client = new Dispatcher.DispatcherClient(channel);
                WeakReferenceMessenger.Default.Register<MsgDispatcher, string>(this, Name, async (r, m) => await SendMessageAsync(m));
            }
            else
                Console.WriteLine("Error");
        }

        public async Task<string> SendMessageAsync(MsgDispatcher mex) // dopo crea class/struct per message
        {
            if (!CreateChannel()) Console.WriteLine("Error");
            Msg input = new Msg();
            input.DateTime = mex.dateTime.ToString();
            input.SentMessage = mex.msg;
            input.MId = mex.mId;
            input.UserId = mex.userId;
            try
            {
                var reply = await client.SendMsgAsync(input);
                return reply.Response.ToString();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return ex.Message;
            }
           
            
        }

        private bool CreateChannel()
        {
            if (channel != null)
                return true;

            if (string.IsNullOrWhiteSpace(Ip))
                return false;

            if (!(Port > 0 && Port < Math.Pow(2, 16) - 1))
                return false;

            channel = GrpcChannel.ForAddress($"{(Https ? "https" : "http")}://{Ip}:{Port}", new GrpcChannelOptions
            {
                HttpHandler = handler,
                
            });
            return true;
        }

    }
}
