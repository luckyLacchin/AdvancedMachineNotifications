using Microsoft.EntityFrameworkCore;
using TelegramClientServer.Models;

namespace TelegramClientServer.Data
{
    public class ChatterRepo : IChatterRepo
    {
        private readonly TelegramContext db;
        public ChatterRepo(TelegramContext dbContext)
        {
            db = dbContext;
        }

        public async Task<Chatter> CreateChatter(string chatId, string status)
        {
            Chatter chatter = new Chatter(chatId, status);
            var result = await db.AddAsync(chatter); 
            return result.Entity;
        }

        public void DeleteChatter(Chatter chatter)
        {
            if (chatter == null)
            {
                throw new ArgumentNullException();
            }
            db.Chatters.Remove(chatter);
        }

        public async Task<Chatter> GetChatter(int id)
        {
            return await db.Chatters.Where(m => m.Id == id).SingleOrDefaultAsync();
        }

        public async Task SaveChanges()
        {
            await db.SaveChangesAsync();
        }

        public void updateChatter(Chatter chatterToUpdate, Chatter chatter)
        {
            db.Entry(chatterToUpdate).CurrentValues.SetValues(chatter);
        }
    }
}
