using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using DemoAPIBot.Models;

namespace DemoAPIBot.Data
{
    public class SubRepo : ISubRepo
    {
        private readonly DemoContext db;
        private readonly ILogger logger;
        public SubRepo(DemoContext dbContext, ILogger<MacchinaRepo> _logger)
        {
            db = dbContext;
            logger = _logger;
        }

        public async Task<Sub> CreateSub()
        {
            Sub sub = new Sub(); 
            var result = await db.AddAsync<Sub>(sub); //dopo devo ricordarmi di salvare il db!
            return result.Entity;
        }

        public void DeleteSub(Sub sub)
        {
            if (sub == null)
            {
                logger.LogError("Sub is null");
            }
            db.Subs.Remove(sub); //dopo devo ricordarmi di salvare l'eliminazione del sub! Purtroppo non esite RemoveAsync!
        }

        public async Task<IEnumerable<Macchina>> GetAllMachinesBySubId(int id) //potrei anche mettere sub qua invece che "int id"
        {
            return await db.SubMachines.Where(m => m.SubId == id).Include(s => s.MacchinaRef).ThenInclude(s => s.subMachines).Select(mc => mc.MacchinaRef).ToListAsync();
        }

        public async Task<Sub?> GetSubById(int id)
        {
            return await db.Subs.Where(s => s.Id == id).Include(s => s.subMachines).SingleOrDefaultAsync();
            //potevo anche faer d.Subs.FirstOrDefaultAsync(s => s.Id = id);, ma con SingleAsync dovrebbe essere un pelo meglio perché mi dà
            //anche l'errore in caso fossero più di uno!
        }

        public async Task SaveChanges()
        {
            await db.SaveChangesAsync();
        }
        public void updateItem(Sub subToUpdate, Sub sub)
        {
            db.Entry(subToUpdate).CurrentValues.SetValues(sub);
        }
    }
}
