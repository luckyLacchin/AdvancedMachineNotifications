using DemoAPIBot.Data;
using DemoAPIBot.Dtos.MachineDto;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace DemoAPIBot.Endpoints.Machine
{
    [Authorize]
    public class GetMachineById : EndpointWithoutRequest
    {
        public IMacchinaRepo repo;
        public AutoMapper.IMapper mapper;
        private readonly ILogger <GetMachineById> logger;
        public GetMachineById(ILogger<GetMachineById> _logger,IMacchinaRepo _repo, AutoMapper.IMapper _mapper)
        {
            this.repo = _repo;
            this.mapper = _mapper;
            logger = _logger;
        }

        public override void Configure()
        {
            Verbs(Http.GET);
            Routes("api/v1/machine/{mId}");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            logger.LogInformation("Enter GetMachine api");
            try
            {
                string mId = Route<string>("mId");
                var machine = await repo.GetMachine(mId);
                if (machine == null)
                {
                    logger.LogWarning("Machine does not exist, cannot fetch it");
                    await SendNotFoundAsync();
                }
                else
                    await SendOkAsync(mapper.Map<ReadMacchinaDto>(machine));

            }
            catch (Exception ex)
            {
                await SendErrorsAsync();
            }
        }
    }
}
