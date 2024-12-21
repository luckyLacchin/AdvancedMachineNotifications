using DemoAPIBot.Dtos.SubMachineDto;
using DemoAPIBot.Models;

namespace DemoAPIBot.Dtos.SubDto
{
    public class UpdateSubDto
    {
        public int Id { get; set; }
        public List <ReadSubMachineDto> subMachines { get; set; }
        //attualmente UpdateSubDto e ReadSubDto sono uguali,
    }
}
