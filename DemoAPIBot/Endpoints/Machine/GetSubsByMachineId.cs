using DemoAPIBot.Data;
using DemoAPIBot.Dtos.SubDto;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;

namespace DemoAPIBot.Endpoints.Machine
{
    [Authorize]
    public class GetSubsByMachineId : EndpointWithoutRequest
    {
        public IMacchinaRepo repo;
        public AutoMapper.IMapper mapper;
        private readonly ILogger<GetSubsByMachineId> logger;
        public GetSubsByMachineId(ILogger<GetSubsByMachineId> _logger, IMacchinaRepo _repo, AutoMapper.IMapper _mapper)
        {
            this.repo = _repo;
            this.mapper = _mapper;
            logger = _logger;
        }

        public override void Configure()
        {
            Verbs(Http.GET);
            Routes("api/v1/GetSubsByMachineId/{mId}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            logger.LogInformation("Enter GetSubsByMachineId api");
            try
            {
                string mId = Route<string>("mId");
                var machine = await repo.GetMachine(mId);
                if (machine == null)
                {
                    logger.LogWarning("Machine does not exist, cannot fetch the subs");
                    await SendNotFoundAsync();
                }
                else
                {
                    var subs = await repo.GetAllSubsByMId(mId); //una volta che ci viene data la sub, dovrebbe essere impossibile che non si trovi nemmeno una macchina
                    await SendOkAsync(mapper.Map<IEnumerable<ReadSubDto>>(subs));
                }
            }
            catch
            {
                await SendErrorsAsync();
            }
        }
    }
}
