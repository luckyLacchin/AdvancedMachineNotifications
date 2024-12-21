using DemoAPIBot.Data;
using DemoAPIBot.Dtos.LoginDto;
using DemoAPIBot.Endpoints.Login_Refresh;
using DemoAPIBot.Services;
using FastEndpoints;
using FastEndpoints.Security;
using System.IdentityModel.Tokens.Jwt;

namespace DemoAPIBot.Endpoints.Login
{
    public class UserLoginEndpoint : Endpoint<LoginRequest, LoginResponse>
    {
        private IConfiguration configuration;
        private DemoContext db;
        private readonly ILogger<UserLoginEndpoint> logger;
        public UserLoginEndpoint(ILogger<UserLoginEndpoint> _logger,IConfiguration _configuration, DemoContext _db)
        {
            configuration = _configuration;
            db = _db;
            logger = _logger;  
        }

        public override void Configure()
        {
            Post("api/v1/login");
            AllowAnonymous();
        }

        public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
        {
            logger.LogInformation("Enter login api.");
            if (AuthService.CredentialsAreValid(req.Username, req.Password))
            {

                Response = await CreateTokenWith<MyTokenService>(req.Username, p =>
                {
                    if (db.Users.Where(x => x.Username == req.Username).Any())
                    {
                        logger.LogInformation("Login of an admin.");
                        p.Roles.Add("admin");
                    }
                    else
                    {
                        logger.LogInformation("Login of a normal user.");
                        p.Roles.Add("machine");
                    }
                });
            }
            else
                await SendErrorsAsync();
        }
    }
}
