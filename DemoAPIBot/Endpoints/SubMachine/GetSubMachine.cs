using DemoAPIBot.Data;
using DemoAPIBot.Dtos.SubMachineDto;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;

namespace DemoAPIBot.Endpoints.SubMachine
{
    [Authorize]
    public class GetSubMachine : EndpointWithoutRequest
    {
        public ISubMachineRepo repo;
        public AutoMapper.IMapper mapper;
        ILogger<GetSubMachine> logger;
        public GetSubMachine(ILogger<GetSubMachine> _logger, ISubMachineRepo _repo, AutoMapper.IMapper _mapper)
        {
            this.repo = _repo;
            this.mapper = _mapper;
            logger = _logger;
        }

        public override void Configure()
        {
            Verbs(Http.GET);
            Routes("api/v1/sub/machine/{id}/{mId}");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            logger.LogInformation("Enter GetSubMachine api");
            int id = Route<int>("id");
            string mId = Route<string>("mId");
            var subMachine = await repo.GetSubMachine(id, mId);
            if (subMachine == null)
            {
                logger.LogWarning("SubMachine is null, cannot be fetched");
                await SendNotFoundAsync();
            }
            else
                await SendOkAsync(mapper.Map<ReadSubMachineDto>(subMachine));
        }
    }
}
