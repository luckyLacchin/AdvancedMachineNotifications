using AutoMapper;
using DemoAPIBot.Data;
using DemoAPIBot.Dtos.SubMachineDto;
using DemoAPIBot.Models;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace DemoAPIBot.Endpoints.SubMachine
{
    [Authorize(Roles = "admin")]
    public class PutSubMachine : Endpoint<UpdateSubMachineDto>
    {
        public ISubMachineRepo repo;
        public AutoMapper.IMapper mapper;
        private readonly ILogger <PutSubMachine> logger;
        public PutSubMachine(ILogger<PutSubMachine> _logger,ISubMachineRepo _repo, AutoMapper.IMapper _mapper)
        {
            this.repo = _repo;
            this.mapper = _mapper;
            logger = _logger;
        }

        public override void Configure()
        {
            Verbs(Http.PUT);
            Routes("api/v1/sub/machine/");
        }

        public override async Task HandleAsync(UpdateSubMachineDto subMachine, CancellationToken ct)
        {
            logger.LogInformation("Enter PutSubMachine api");
            try
            {
                if (subMachine == null)
                {
                    logger.LogWarning("SubMachine is null, cannot update");
                    await SendErrorsAsync();
                }
                var subMachineToUpdate = await repo.GetSubMachine(subMachine.SubId, subMachine.MacchinaId);
                if (subMachineToUpdate == null)
                {
                    logger.LogWarning("SubMachine does not exist, cannot be updated");
                    await SendNotFoundAsync();
                }
                else
                {
                    var subMachineUpdating = mapper.Map<DemoAPIBot.Models.SubMachine>(subMachine);
                    repo.updateItem(subMachineToUpdate, subMachineUpdating);
                    await repo.SaveChanges();
                    await SendOkAsync();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error in connecting to the db");
                await SendErrorsAsync();
            }
        }
    }
}
