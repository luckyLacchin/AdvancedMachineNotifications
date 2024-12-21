using DemoAPIBot.Data;
using DemoAPIBot.Dtos.SubDto;
using DemoAPIBot.Models;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DemoAPIBot.Endpoints.Sub
{
    [Authorize(Roles = "admin")]
    public class PutSub : Endpoint<UpdateSubDto>
    {
        public ISubRepo repo;
        public AutoMapper.IMapper mapper;
        private readonly ILogger<PutSub> logger;
        public PutSub(ILogger<PutSub> _logger,ISubRepo _repo, AutoMapper.IMapper _mapper)
        {
            this.repo = _repo;
            this.mapper = _mapper;
            logger = _logger;
        }

        public override void Configure()
        {
            Verbs(Http.PUT);
            Routes("api/v1/sub");
        }

        public override async Task HandleAsync(UpdateSubDto sub, CancellationToken ct)
        {
            logger.LogInformation("Enter PutSub api");
            try
            {
                var subToUpdate = await repo.GetSubById(sub.Id);
                if (subToUpdate == null)
                {
                    logger.LogWarning("SubToUpdate does not exist");
                    await SendNotFoundAsync();
                }
                else
                {
                    var updatingSub = mapper.Map<DemoAPIBot.Models.Sub>(sub);
                    repo.updateItem(subToUpdate, updatingSub);
                    await repo.SaveChanges();
                    await SendOkAsync();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error connecting to the db.");
                await SendErrorsAsync();
            }
        }
    }
}
