using DemoAPIBot.Models;

namespace DemoAPIBot.Dtos.SubMachineDto
{
    public class UpdateSubMachineDto
    {
        public int SubId { get; set; }
        public Sub SubRef { get; set; }
        public string MacchinaId { get; set; }
        public Macchina MacchinaRef { get; set; }
        public int levelPriority { get; set; }
        public string serviceDispatcher { get; set; }
    }
}
