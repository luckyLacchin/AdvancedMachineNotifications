using FastEndpoints.Security;

namespace DemoAPIBot.Dtos.LoginDto
{
    public class LoginResponse : TokenResponse
    {
        //ideally should be using something like nodatime to convert to the local time zone of the client app
        public string AccessTokenExpiry => AccessExpiry.ToLocalTime().ToString();

        public int RefreshTokenValidityMinutes => (int)RefreshExpiry.Subtract(DateTime.UtcNow).TotalMinutes;
    }
}
