using DemoAPIBot.Models;

namespace DemoAPIBot.Data
{
    public interface ISubRepo
    {
        Task SaveChanges();
        Task <Sub?> GetSubById (int id);
        Task <Sub>CreateSub();
        Task<IEnumerable<Macchina>> GetAllMachinesBySubId(int id);

        void DeleteSub (Sub sub);
        public void updateItem(Sub subToUpdate, Sub sub);


    }
}
