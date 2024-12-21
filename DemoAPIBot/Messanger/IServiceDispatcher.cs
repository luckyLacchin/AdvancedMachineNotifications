using AutoMapper.Configuration.Conventions;
using DemoAPIBot.Data;
using DemoServiceProto;

namespace DemoAPIBot.Messanger
{
    public interface IServiceDispatcher
    {
        string Ip { get; }
        string Name { get; }
        int Port { get; }

        void SetService(ServerAlive request);
        Task<string> SendMessageAsync(MsgDispatcher mex);
    }
}