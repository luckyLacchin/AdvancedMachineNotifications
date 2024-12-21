using DemoAPIBot.Models;

namespace DemoAPIBot.Data
{
    public interface ISubMachineRepo
    {
        Task SaveChanges();
        Task<SubMachine?> GetSubMachine(int id, string mId);
        Task<SubMachine> CreateSubMachine(string mId, int subId, string serviceDispatcher);
        public void DeleteSubMachine(SubMachine subMachine);
        public void updateItem(SubMachine subMachineToUpdate, SubMachine subMachine);

    }
}
