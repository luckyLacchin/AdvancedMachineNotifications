using Microsoft.EntityFrameworkCore;
using DemoAPIBot.Models;
using System.Linq.Expressions;

namespace DemoAPIBot.Data
{
    public class SubMachineRepo : ISubMachineRepo
    {
        private readonly DemoContext db;
        private readonly IMacchinaRepo _repo_macchina;
        private readonly ISubRepo _repo_sub;
        private readonly ILogger<SubMachineRepo> _logger;
        public SubMachineRepo(ILogger<SubMachineRepo> logger, DemoContext dbContext, IMacchinaRepo repo_macchina, ISubRepo repo_sub)
        {
            db = dbContext;
            _repo_macchina = repo_macchina;
            _repo_sub = repo_sub;
            _logger = logger;
        }
        public async Task<SubMachine> CreateSubMachine(string mId, int subId,string serviceDispatcher)
        {
            try
            {
                Macchina exists_macchina = await _repo_macchina.GetMachine(mId);
                Sub exists_sub = await _repo_sub.GetSubById(subId);
                if (exists_macchina == null || exists_sub == null) {
                    return null;
                }
                SubMachine subMachine = new SubMachine(subId,mId,serviceDispatcher);
                var result = await db.AddAsync(subMachine);
                return result.Entity;
            }
            catch (Exception ex) { 
                //in questo catch gestisco la possibilità in cui ho un errore nel fare il getMachine o getSubId
                _logger.LogError(ex.Message);
                return null;

            }
        }

        public void DeleteSubMachine(SubMachine subMachine)
        {
            if (subMachine == null)
            {
                _logger.LogError("SubMachine is null");
            }
            else
                db.SubMachines.Remove(subMachine);        
        }

        public async Task<SubMachine?> GetSubMachine(int id, string mId)
        {
            return await db.SubMachines.Where(s => s.SubId == id && s.MacchinaId == mId)
                .Include(s => s.SubRef).ThenInclude(s => s.subMachines)
                .Include(s => s.MacchinaRef)
                .ThenInclude(s => s.subMachines)
                .SingleOrDefaultAsync();
        }

        public async Task SaveChanges()
        {
            await db.SaveChangesAsync();
        }

        public void updateItem (SubMachine subMachineToUpdate, SubMachine subMachine)
        {
            db.Entry(subMachineToUpdate).CurrentValues.SetValues(subMachine);
        }
    }
}
