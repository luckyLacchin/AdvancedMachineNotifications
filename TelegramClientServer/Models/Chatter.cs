using Microsoft.EntityFrameworkCore;

namespace TelegramClientServer.Models
{
    [Index(nameof(Id))]
    public class Chatter //per ora mettiamo tutto in una relation, in futuro potrei creare
        //più relation/creare delle relationships.
    {

        public Chatter(string chatId, string status)
        {
            this.chatId = chatId;
            this.status = status;
        }


        public Chatter(string chatId, string status, string mId) : this(chatId, status)
        {
            this.mId = mId;
        }

        public int Id { get; set; }
        public string chatId { get; set; }
        public string status { get; set; }
        public string mId { get; set; }
    }
}
