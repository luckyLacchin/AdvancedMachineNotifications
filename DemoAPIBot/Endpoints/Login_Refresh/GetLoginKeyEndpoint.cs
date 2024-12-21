using DemoAPIBot.Dtos.LoginDto;
using FastEndpoints;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DemoAPIBot.Endpoints
{
    public class GetLoginKeyEndpoint : EndpointWithoutRequest<GetLoginResponse>
    {
        ILogger <GetLoginKeyEndpoint> logger;

        public GetLoginKeyEndpoint(ILogger<GetLoginKeyEndpoint> logger)
        {
            this.logger = logger;
        }

        public override void Configure()
        {
            Verbs(Http.GET);
            Routes("api/v1/GetLoginKey");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            logger.LogInformation("Enter GetLoginKey api.");
            await SendOkAsync(new GetLoginResponse { dateTime = DateTime.UtcNow});
        }
    }
}
