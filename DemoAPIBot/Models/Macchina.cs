using System.ComponentModel.DataAnnotations;
using System.Reflection;
using DemoAPIBot.Models;

namespace DemoAPIBot.Models
{
    public class Macchina
    {
        public Macchina()
        {
        }


        public Macchina(string mId)
        {
            this.mId = mId;
        }

        public Macchina(string mId, string model)
        {
            this.mId = mId;
            this.model = model;
        }
        [Key]
        public string mId { get; set; }
        public string? model { get; set; }

        public List<SubMachine> subMachines { get; set; }

    }
}
