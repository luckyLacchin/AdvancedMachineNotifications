using System.Security.Cryptography;
namespace DemoAPIBot.Dtos.MachineDto
{
    public class TokenMachine
    {
        public TokenMachine()
        {
            token = CreateSecureRandomString();
            Console.WriteLine($"token = {token}");

            timeToExpire = new TimeSpan (0,0,5,0);
        }

        public string token { get; set; }
        public TimeSpan timeToExpire { get; set; }
        public static string CreateSecureRandomString(int count = 8) =>
            Convert.ToBase64String(RandomNumberGenerator.GetBytes(count));
    }
}
