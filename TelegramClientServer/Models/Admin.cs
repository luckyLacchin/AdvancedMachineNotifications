namespace TelegramClientServer.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string Username { get; set; } //questo potrei anche non metterlo, lo metto solo per specificare chi sia l'admin
        public string chatId { get; set; }

    }
    //Questa relation serve per poter salvare chi siano gli admin, con il loro chatId, del bot.
    //Questi potranno visualizzare comandi aggiuntivi.
}
