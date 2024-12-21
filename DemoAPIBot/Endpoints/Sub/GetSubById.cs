using DemoAPIBot.Data;
using DemoAPIBot.Dtos.SubDto;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace DemoAPIBot.Endpoints.Sub
{
    [Authorize]
    public class GetSubById : EndpointWithoutRequest<ReadSubDto>
    {
        public ISubRepo repo;
        public AutoMapper.IMapper mapper;
        private readonly ILogger<GetSubById> logger;
        public GetSubById(ILogger<GetSubById> _logger,ISubRepo _repo, AutoMapper.IMapper _mapper)
        {
            this.repo = _repo;
            this.mapper = _mapper;
            logger = _logger;
        }

        public override void Configure()
        {
            Verbs(Http.GET);
            Routes("api/v1/sub/{id}");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            logger.LogInformation("Enter GetSubById api");
            try
            {
                int id = Route<int>("id");
                var sub = await repo.GetSubById(id);
                if (sub == null)
                {
                    logger.LogWarning("Sub does not exist, cannot be fetched");
                    await SendNotFoundAsync(); //è il caso in cui l'id dato è sbagliato e non ho ottenuto un sub!
                }
                else
                    await SendCreatedAtAsync("api/v1/sub/{id}", null, mapper.Map<ReadSubDto>(sub));
            }
            catch (Exception ex)
            {
                logger.LogError("Error connecting to the db");
                await SendErrorsAsync();
            }
        }
    }
}
