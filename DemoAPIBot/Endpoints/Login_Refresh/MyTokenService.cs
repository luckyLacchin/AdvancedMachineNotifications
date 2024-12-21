using DemoAPIBot.Data;
using DemoAPIBot.Dtos.LoginDto;
using FastEndpoints;
using FastEndpoints.Security;

namespace DemoAPIBot.Endpoints.Login_Refresh
{
    public class MyTokenService : RefreshTokenService<TokenRequest, LoginResponse>
    {
        DemoContext db;
        public MyTokenService(IConfiguration config, DemoContext _db)
        {
            Setup(o =>
            {
                o.TokenSigningKey = config["Jwt:Key"];
                o.AccessTokenValidity = TimeSpan.FromMinutes(5);
                o.RefreshTokenValidity = TimeSpan.FromHours(4);
                o.Endpoint("/api/v1/refresh-token", ep =>
                {
                    ep.Summary(s => s.Summary = "this is the refresh token endpoint");
                });
            });
            db = _db;
        }

        public override async Task PersistTokenAsync(LoginResponse response)
        {
            await DataToken.StoreToken(db, response.UserId, response.RefreshExpiry, response.RefreshToken);

            // this method will be called whenever a new access/refresh token pair is being generated.
            // store the tokens and expiry dates however you wish for the purpose of verifying
            // future refresh requests.        
        }

        public override async Task RefreshRequestValidationAsync(TokenRequest req)
        {
            if (!await DataToken.TokenIsValid(db, req.UserId, req.RefreshToken))
                AddError(r => r.RefreshToken, "Refresh token is invalid!");

            // validate the incoming refresh request by checking the token and expiry against the
            // previously stored data. if the token is not valid and a new token pair should
            // not be created, simply add validation errors using the AddError() method.
            // the failures you add will be sent to the requesting client. if no failures are added,
            // validation passes and a new token pair will be created and sent to the client.        
        }

        public override async Task SetRenewalPrivilegesAsync(TokenRequest request, UserPrivileges privileges)
        {
            if (db.Users.Where(x => x.Username == request.UserId).Any())
                privileges.Roles.Add("admin");
            else
                privileges.Roles.Add("machine");
            // specify the user privileges to be embedded in the jwt when a refresh request is
            // received and validation has passed. this only applies to renewal/refresh requests
            // received to the refresh endpoint and not the initial jwt creation.        
        }
    }
}
