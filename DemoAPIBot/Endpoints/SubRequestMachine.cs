using DemoAPIBot.Data;
using DemoAPIBot.Dtos.MachineDto;
using DemoAPIBot.Models;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace DemoAPIBot.Endpoints
{
    [Authorize]
    public class SubRequestMachine : Endpoint<CreateMacchinaDto>
    {
        DemoContext db;
        private readonly ILogger<SubRequestMachine> logger;
        public SubRequestMachine(ILogger<SubRequestMachine> _logger,DemoContext _db)
        {
            db = _db;
            logger = _logger;
        }

        public override void Configure()
        {
            Verbs(Http.POST);
            Routes("api/v1/subRequest/");
        }

        public override async Task HandleAsync(CreateMacchinaDto req, CancellationToken ct)
        {
            logger.LogInformation("Enter the SubRequestMachine api");
            if (req != null)
            {
                TokenMachine tokenMachine = new TokenMachine();
                InProgressSub inProgress = new InProgressSub();
                inProgress.mId = req.mId;
                inProgress.timeSubRequest = DateTime.UtcNow;
                inProgress.token = tokenMachine.token;


                try
                {
                    await db.AddAsync(inProgress);
                    await db.SaveChangesAsync();
                    await SendOkAsync(tokenMachine); //restituisco il tempo massimo per fare la subscription
                }
                catch
                {
                    logger.LogError("Error connecting to the db");
                    await SendErrorsAsync();
                }
            }
            else
            {
                logger.LogWarning("CreateMachine is null, cannot be created");
                await SendErrorsAsync();
            }
        }
    }
}
