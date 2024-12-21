using DemoAPIBot.Dtos.SubMachineDto;
using DemoAPIBot.Models;

namespace DemoAPIBot.Dtos.SubDto
{
    public class ReadSubDto
    {
        public int Id { get; set; }
        public List<ReadSubMachineDto> subMachines { get; set; }

    }
}
