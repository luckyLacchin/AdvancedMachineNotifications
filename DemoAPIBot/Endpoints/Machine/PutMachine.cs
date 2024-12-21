using AutoMapper;
using DemoAPIBot.Data;
using DemoAPIBot.Dtos.MachineDto;
using DemoAPIBot.Models;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using System.Reflection.PortableExecutable;

namespace DemoAPIBot.Endpoints.Machine
{
    [Authorize(Roles = "admin")]
    public class PutMachine : Endpoint<UpdateMacchinaDto>
    {
        public IMacchinaRepo repo;
        public AutoMapper.IMapper mapper;
        private readonly ILogger<PutMachine> logger;
        public PutMachine(ILogger<PutMachine> _logger,IMacchinaRepo _repo, AutoMapper.IMapper _mapper)
        {
            this.repo = _repo;
            this.mapper = _mapper;
            logger = _logger;
        }

        public override void Configure()
        {
            Verbs(Http.PUT);
            Routes("api/v1/machine/");
        }

        public override async Task HandleAsync(UpdateMacchinaDto machine, CancellationToken ct)
        {
            logger.LogInformation("Enter PutMachine api");
            try
            {

                if (machine == null)
                {
                    logger.LogWarning("Machine is null, cannot be updated");
                    await SendErrorsAsync();
                }
                var machineToUpdate = await repo.GetMachine(machine.mId);
                if (machineToUpdate == null)
                {
                    logger.LogWarning("Machine does not exist, cannot be updated");
                    await SendNotFoundAsync();
                }
                else
                {
                    var machineUpdating = mapper.Map<Macchina>(machine);
                    repo.updateItem(machineToUpdate, machineUpdating);
                    await repo.SaveChanges();
                    await SendOkAsync();
                }
            }
            catch (Exception ex)
            {
                await SendErrorsAsync();
            }
        }
    }
}
