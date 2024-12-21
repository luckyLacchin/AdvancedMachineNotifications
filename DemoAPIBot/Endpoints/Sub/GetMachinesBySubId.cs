
using DemoAPIBot.Data;
using DemoAPIBot.Dtos.MachineDto;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace DemoAPIBot.Endpoints
{
    [Authorize]
    public class GetMachinesBySubId : EndpointWithoutRequest<IEnumerable<ReadMacchinaDto>>
    {
        public ISubRepo repo;
        public AutoMapper.IMapper mapper;
        private readonly ILogger<GetMachinesBySubId> logger;
        public GetMachinesBySubId(ILogger<GetMachinesBySubId> _logger,ISubRepo _repo, AutoMapper.IMapper _mapper)
        {
            this.repo = _repo;
            this.mapper = _mapper;
            logger = _logger;

        }

        public override void Configure()
        {
            Verbs(Http.GET);
            Routes("api/v1/GetMachinesBySubId/{id}");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            logger.LogInformation("Enter GetMachinesBySubId api");
            try
            {
                int id = Route<int>("id");
                var sub = await repo.GetSubById(id);
                if (sub == null)
                {
                    logger.LogWarning("Sub does not exist");
                    await SendNotFoundAsync();
                }
                else
                {
                    var machines = await repo.GetAllMachinesBySubId(id); //una volta che ci viene data la sub, dovrebbe essere impossibile che non si trovi nemmeno una macchina
                    await SendCreatedAtAsync("api/v1/GetMachinesBySubId/{id}", null, mapper.Map<IEnumerable<ReadMacchinaDto>>(machines));
                }
            }
            catch
            {
                logger.LogError("Error in connecting to the db");
                await SendErrorsAsync();
            }
        }
    }
}
