namespace DemoAPIBot.Dtos.LoginDto
{
    public class LoginRequest
    {
        public string Username { get; set; } //l'username potrebbe essere un utente come un macchinario (mId)!
        public string Password { get; set; } // username+datetime

    }
}
