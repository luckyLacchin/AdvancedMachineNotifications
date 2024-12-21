using Microsoft.EntityFrameworkCore;

namespace DemoAPIBot.Models
{
    [Index(nameof(Id))]
    public class InProgressSub
    {
        public int Id { get; set; }
        public string mId { get; set; }
        public DateTime timeSubRequest { get; set; }
        public string token { get; set; }
    }
}
