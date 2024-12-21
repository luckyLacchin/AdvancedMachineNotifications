using DemoAPIBot.Models;

namespace DemoAPIBot.Dtos.MachineDto
{
    public class UpdateMacchinaDto
    {
        public string mId { get; set; }
        public string model { get; set; }
        public List<SubMachine> subMachines { get; set; }

    }
}
