using DemoAPIBot.Dtos.SubMachineDto;

namespace DemoAPIBot.Dtos.MachineDto
{
    public class ReadMacchinaDto
    {
        public string mId { get; set; }
        public string model { get; set; }
        public List<ReadSubMachineDto> subMachines { get; set; }
    }
}
