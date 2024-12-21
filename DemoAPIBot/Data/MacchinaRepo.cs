using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using DemoAPIBot.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc.DataAnnotations;

namespace DemoAPIBot.Data
{
    public class MacchinaRepo : IMacchinaRepo
    {
        private readonly DemoContext db;
        private readonly ILogger logger;
        public MacchinaRepo(DemoContext dbContext, ILogger <MacchinaRepo> _logger)
        {
            db = dbContext;
            logger = _logger;
        }
        public async Task<Macchina> CreateMachine(string mId, string model)
        {
            Macchina machine = new Macchina(mId, model);
            var result = await db.AddAsync(machine);
            return result.Entity;
        }

        public void DeleteMachine(Macchina machine)
        {
            if (machine == null)
            {
                logger.LogError("Machine is null");
            }
            else
                db.Machines.Remove(machine);
        }

        public async Task<IEnumerable<Sub>> GetAllSubsByMId(string mId)
        {
            return await db.SubMachines.Where(m => m.MacchinaId == mId).Include(m => m.SubRef).ThenInclude(s => s.subMachines).Select(mc => mc.SubRef).ToListAsync();
        }


        public async Task<Macchina> GetMachine(string mId)
        {
            return await db.Machines.Where(m => m.mId == mId).Include(s => s.subMachines).SingleOrDefaultAsync();
            //il caso in cui possa essere == null lo gestisco nelle API che chiamano questa funzione.
        }


        public async Task SaveChanges()
        {
            await db.SaveChangesAsync();
        }
        public void updateItem(Macchina machineToUpdate, Macchina machine)
        {
            db.Entry(machineToUpdate).CurrentValues.SetValues(machine);
        }
    }
}
