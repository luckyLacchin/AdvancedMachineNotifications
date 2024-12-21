namespace DemoAPIBot.Services
{
    public static class AuthService
    {
        public static bool CredentialsAreValid(string username, string password)
        {
            
            if (DateTimeOffset.TryParse(password.Substring(username.Length), out DateTimeOffset datetime))
            {
                if (DateTime.UtcNow.Subtract(datetime.DateTime) < TimeSpan.FromSeconds(60))
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
    }
}
