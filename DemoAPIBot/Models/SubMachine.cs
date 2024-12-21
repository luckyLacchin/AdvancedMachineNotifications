using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace DemoAPIBot.Models
{
    [PrimaryKey(nameof(SubId), nameof(MacchinaId), nameof(serviceDispatcher))]
    public class SubMachine
    {
        public SubMachine()
        {
        }

        public SubMachine(string macchinaId, string serviceDispatcher)
        {
            MacchinaId = macchinaId;
            this.serviceDispatcher = serviceDispatcher;
        }

        public SubMachine(int subId, string macchinaId, string serviceDispatcher)
        {
            SubId = subId;
            MacchinaId = macchinaId;
            this.serviceDispatcher = serviceDispatcher;
        }
        
        public int SubId { get; set; }
        public Sub SubRef { get; set; }
        public string MacchinaId { get; set; }
        public Macchina MacchinaRef { get; set; }
        public int levelPriority { get; set; }
        public string serviceDispatcher { get; set; }
    }
}
