using Newtonsoft.Json.Bson;
using DemoAPIBot.Models;

namespace DemoAPIBot.Data
{
    public interface IMacchinaRepo
    {
        Task SaveChanges();
        Task<Macchina> GetMachine(string mId);
        Task <Macchina> CreateMachine(string mId, string model);
        Task<IEnumerable<Sub>> GetAllSubsByMId(string mId);
        void DeleteMachine (Macchina machine);
        public void updateItem(Macchina machineToUpdate, Macchina machine);
    }
}
