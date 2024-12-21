using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Data;

namespace DemoAPIBot.Data
{
    public class MsgDispatcher //non può essere una struct, in quanto deve essere un tipo riferimento per il metodo Messenger.Register.
    {
        public string msg { get; set; }
        public string typeMsg { get; set; }
        public string mId { get; set; } //mId è l'id della macchina che ha inviato tale Msg, mediante cui
        //andiamo a filtrare le macchine a cui inviare la notifica.
        public string typeDispatcher { get; set; } //questo attributo indica il tipo di Dispatcher
        //con cui stiamo inviando il msg, tipo Telegram, Discord, ecc..
        public int userId { get; set; }
        public DateTime dateTime { get; set; }
    }
}