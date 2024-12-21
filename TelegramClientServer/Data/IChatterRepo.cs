using TelegramClientServer.Models;

namespace TelegramClientServer.Data
{
    public interface IChatterRepo
    {
        Task SaveChanges();
        Task<Chatter> GetChatter(int id);
        Task<Chatter> CreateChatter(string chatId, string status);
        void DeleteChatter(Chatter chatter);
        public void updateChatter(Chatter chatterToUpdate, Chatter chatter);

    }
}
